// SPDX-FileCopyrightText: 2023 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Damage.Components;
using Content.Server.Popups;
using Content.Shared.Damage;
using Robust.Shared.Player;
using Robust.Shared.Random;

namespace Content.Server.Damage.Systems;

/// <summary>
/// Outputs a random pop-up from the strings list when an object receives damage
/// </summary>
public sealed class DamageRandomPopupSystem : EntitySystem
{
    [Dependency] private readonly PopupSystem _popupSystem = default!;
    [Dependency] private readonly IRobustRandom _random = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DamageRandomPopupComponent, DamageChangedEvent>(OnDamageChange);
    }

    private void OnDamageChange(EntityUid uid, DamageRandomPopupComponent component, DamageChangedEvent args)
    {
        _popupSystem.PopupEntity(Loc.GetString(_random.Pick(component.Popups)), uid);
    }
}
