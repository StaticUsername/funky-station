// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Players.RateLimiting;
using Robust.Shared.Player;

namespace Content.Client.Players.RateLimiting;

public sealed class PlayerRateLimitManager : SharedPlayerRateLimitManager
{
    public override RateLimitStatus CountAction(ICommonSession player, string key)
    {
        // TODO Rate-Limit
        // Add support for rate limit prediction
        // I.e., dont mis-predict just because somebody is clicking too quickly.
        return RateLimitStatus.Allowed;
    }

    public override void Register(string key, RateLimitRegistration registration)
    {
    }

    public override void Initialize()
    {
    }
}
