// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

namespace Content.Server.Salvage.Magnet;

/// <summary>
/// Indicates the entity is a salvage target for tracking.
/// </summary>
[RegisterComponent]
public sealed partial class SalvageMagnetTargetComponent : Component
{
    /// <summary>
    /// Entity that spawned us.
    /// </summary>
    [DataField]
    public EntityUid DataTarget;
}
