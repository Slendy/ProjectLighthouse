#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Configuration.ConfigurationCategories;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace LBPUnion.ProjectLighthouse.Extensions;

public static partial class RequestExtensions
{
    static RequestExtensions()
    {
        client = new HttpClient();
    }
    
    #region Mobile Checking

    // yoinked and adapted from https://stackoverflow.com/a/68641796
    [GeneratedRegex("Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini|PlayStation Vita",
        RegexOptions.IgnoreCase | RegexOptions.Multiline,
        "en-US")]
    private static partial Regex MobileCheckRegex();

    public static bool IsMobile(this HttpRequest request) => MobileCheckRegex().IsMatch(request.Headers[HeaderNames.UserAgent].ToString());

    #endregion

    #region Captcha

    private static readonly HttpClient client;

    private static readonly Dictionary<CaptchaType, Uri> captchaUrlMap = new()
    {
        {
            CaptchaType.HCaptcha, new Uri("https://hcaptcha.com/siteverify")
        },
        {
            CaptchaType.ReCaptcha, new Uri("https://www.google.com/recaptcha/api/siteverify")
        },
    };

    [SuppressMessage("ReSharper", "ArrangeObjectCreationWhenTypeNotEvident")]
    private static async Task<bool> verifyCaptcha(ServerConfiguration serverConfig, string? token)
    {
        if (!serverConfig.Captcha.CaptchaEnabled) return true;

        if (token == null) return false;

        List<KeyValuePair<string, string>> payload = new()
        {
            new("secret", serverConfig.Captcha.Secret),
            new("response", token),
        };

        captchaUrlMap.TryGetValue(serverConfig.Captcha.Type, out Uri? url);

        HttpResponseMessage response = await client.PostAsync(url, new FormUrlEncodedContent(payload));

        response.EnsureSuccessStatusCode();

        string responseBody = await response.Content.ReadAsStringAsync();

        // We only really care about the success result, nothing else that hcaptcha sends us, so lets only parse that.
        bool success = bool.Parse(JObject.Parse(responseBody)["success"]?.ToString() ?? "false");
        return success;
    }

    public static async Task<bool> CheckCaptchaValidity(this HttpRequest request, ServerConfiguration serverConfig)
    {
        if (!serverConfig.Captcha.CaptchaEnabled) return true;

        string keyName = serverConfig.Captcha.Type switch
        {
            CaptchaType.HCaptcha => "h-captcha-response",
            CaptchaType.ReCaptcha => "g-recaptcha-response",
            _ => throw new ArgumentOutOfRangeException(nameof(request), @$"Unknown captcha type: {serverConfig.Captcha.Type}"),
        };
            
        bool gotCaptcha = request.Form.TryGetValue(keyName, out StringValues values);
        if (!gotCaptcha) return false;

        return await verifyCaptcha(serverConfig, values[0]);
    }
    #endregion

}