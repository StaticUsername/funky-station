// SPDX-FileCopyrightText: 2024 Aviu00 <93730715+Aviu00@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 John Space <bigdumb421@gmail.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.Examine;
using Content.Shared.Inventory;

namespace Content.Shared.Stunnable;

public sealed class ClothingModifyStunTimeSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ClothingModifyStunTimeComponent, InventoryRelayedEvent<ModifyStunTimeEvent>>(
            OnModifyStunTime);
        SubscribeLocalEvent<ClothingModifyStunTimeComponent, ExaminedEvent>(OnExamined);
    }

    private void OnExamined(Entity<ClothingModifyStunTimeComponent> ent, ref ExaminedEvent args)
    {
        var msg = Loc.GetString("clothing-modify-stun-time-examine",
            ("mod", MathF.Round((1f - ent.Comp.Modifier) * 100)));
        args.PushMarkup(msg);
    }

    private void OnModifyStunTime(Entity<ClothingModifyStunTimeComponent> ent,
        ref InventoryRelayedEvent<ModifyStunTimeEvent> args)
    {
        args.Args.Modifier *= ent.Comp.Modifier;
    }

    public float GetModifier(EntityUid uid)
    {
        var ev = new ModifyStunTimeEvent(1f);
        RaiseLocalEvent(uid, ref ev);
        return ev.Modifier;
    }
}

[ByRefEvent]
public record struct ModifyStunTimeEvent(float Modifier) : IInventoryRelayEvent
{
    public SlotFlags TargetSlots => SlotFlags.WITHOUT_POCKET;
}
