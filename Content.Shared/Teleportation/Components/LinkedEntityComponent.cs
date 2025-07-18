// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 LordEclipse <106132477+LordEclipse@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Teleportation.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.Teleportation.Components;

/// <summary>
///     Represents an entity which is linked to other entities (perhaps portals), and which can be walked through /
///     thrown into to teleport an entity.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(LinkedEntitySystem))]
public sealed partial class LinkedEntityComponent : Component
{
    /// <summary>
    ///     The entities that this entity is linked to.
    /// </summary>
    [DataField, AutoNetworkedField]
    public HashSet<EntityUid> LinkedEntities = new();

    /// <summary>
    ///     Should this entity be deleted if all of its links are removed?
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public bool DeleteOnEmptyLinks;
}

[Serializable, NetSerializable]
public enum LinkedEntityVisuals : byte
{
    HasAnyLinks
}
