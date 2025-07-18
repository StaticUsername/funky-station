// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 username <113782077+whateverusername0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Content.Shared.Chemistry.Components.SolutionManager;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Fluids.Components;
using Content.Shared.Heretic.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server.Heretic.Ritual;

public sealed partial class RitualReagentPuddleBehavior : RitualCustomBehavior
{
    protected EntityLookupSystem _lookup = default!;

    [DataField] public ProtoId<ReagentPrototype>? Reagent;

    private List<EntityUid> uids = new();

    public override bool Execute(RitualData args, out string? outstr)
    {
        outstr = null;

        if (Reagent == null)
            return true;

        _lookup = args.EntityManager.System<EntityLookupSystem>();

        var lookup = _lookup.GetEntitiesInRange(args.Platform, .75f);

        foreach (var ent in lookup)
        {
            if (!args.EntityManager.TryGetComponent<PuddleComponent>(ent, out var puddle))
                continue;

            if (puddle.Solution == null)
                continue;

            var soln = puddle.Solution.Value;

            if (!soln.Comp.Solution.ContainsPrototype(Reagent))
                continue;

            uids.Add(ent);
        }

        if (uids.Count == 0)
        {
            outstr = Loc.GetString("heretic-ritual-fail-reagentpuddle", ("reagentname", Reagent!));
            return false;
        }

        return true;
    }

    public override void Finalize(RitualData args)
    {
        foreach (var uid in uids)
            args.EntityManager.QueueDeleteEntity(uid);
        uids = new();
    }
}
