#!/bin/bash

# Developer script to create EntityFramework database migrations
#
# $1: Name of the migration, e.g. SwitchToPermissionLevels
# Invoked manually

cd $(git rev-parse --show-toplevel) || exit 1
dotnet ef migrations add "$1" --project ./ProjectLighthouse -s ./ProjectLighthouse.Servers.GameServer 