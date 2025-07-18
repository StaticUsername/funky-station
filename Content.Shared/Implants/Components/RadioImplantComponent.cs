// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Radio;
using Robust.Shared.Prototypes;

namespace Content.Shared.Implants.Components;

/// <summary>
/// Gives the user access to a given channel without the need for a headset.
/// </summary>
[RegisterComponent]
public sealed partial class RadioImplantComponent : Component
{
    /// <summary>
    /// The radio channel(s) to grant access to.
    /// </summary>
    [DataField(required: true)]
    public HashSet<ProtoId<RadioChannelPrototype>> RadioChannels = new();

    /// <summary>
    /// The radio channels that have been added by the implant to a user's ActiveRadioComponent.
    /// Used to track which channels were successfully added (not already in user)
    /// </summary>
    /// <remarks>
    /// Should not be modified outside RadioImplantSystem.cs
    /// </remarks>
    [DataField]
    public HashSet<ProtoId<RadioChannelPrototype>> ActiveAddedChannels = new();

    /// <summary>
    /// The radio channels that have been added by the implant to a user's IntrinsicRadioTransmitterComponent.
    /// Used to track which channels were successfully added (not already in user)
    /// </summary>
    /// <remarks>
    /// Should not be modified outside RadioImplantSystem.cs
    /// </remarks>
    [DataField]
    public HashSet<ProtoId<RadioChannelPrototype>> TransmitterAddedChannels = new();
}
