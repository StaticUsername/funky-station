// SPDX-FileCopyrightText: 2023 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Alert;
using Content.Shared.Ninja.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;

namespace Content.Shared.Ninja.Components;

/// <summary>
/// Component placed on a mob to make it a space ninja, able to use suit and glove powers.
/// Contains ids of all ninja equipment.
/// </summary>
[RegisterComponent, NetworkedComponent, AutoGenerateComponentState]
[Access(typeof(SharedSpaceNinjaSystem))]
public sealed partial class SpaceNinjaComponent : Component
{
    /// <summary>
    /// Currently worn suit
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? Suit;

    /// <summary>
    /// Currently worn gloves, if enabled.
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? Gloves;

    /// <summary>
    /// Bound katana, set once picked up and never removed
    /// </summary>
    [DataField, AutoNetworkedField]
    public EntityUid? Katana;

    /// <summary>
    /// Objective to complete after calling in a threat.
    /// </summary>
    [DataField]
    public EntProtoId TerrorObjective = "TerrorObjective";

    /// <summary>
    /// Objective to complete after setting everyone to arrest.
    /// </summary>
    [DataField]
    public EntProtoId MassArrestObjective = "MassArrestObjective";

    /// <summary>
    /// Objective to complete after the spider charge detonates.
    /// </summary>
    [DataField]
    public EntProtoId SpiderChargeObjective = "SpiderChargeObjective";

    /// <summary>
    /// Alert to show for suit power.
    /// </summary>
    [DataField]
    public ProtoId<AlertPrototype> SuitPowerAlert = "SuitPower";
}
