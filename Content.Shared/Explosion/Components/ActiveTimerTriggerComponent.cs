// SPDX-FileCopyrightText: 2024 Aexxie <codyfox.077@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Robust.Shared.Audio;
using Robust.Shared.GameStates;

namespace Content.Shared.Explosion.Components;

/// <summary>
///     Component for tracking active trigger timers. A timers can activated by some other component, e.g. <see cref="OnUseTimerTriggerComponent"/>.
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class ActiveTimerTriggerComponent : Component
{
    [DataField] public float TimeRemaining;

    [DataField] public EntityUid? User;

    [DataField] public float BeepInterval;

    [DataField] public float TimeUntilBeep;

    [DataField] public SoundSpecifier? BeepSound;
}
