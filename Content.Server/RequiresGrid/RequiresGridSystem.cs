// SPDX-FileCopyrightText: 2024 HoofedEar <HoofedEar@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Destructible;

namespace Content.Server.RequiresGrid;

public sealed class RequiresGridSystem : EntitySystem
{
    [Dependency] private readonly DestructibleSystem _destructible = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RequiresGridComponent, EntParentChangedMessage>(OnEntParentChanged);
    }

    private void OnEntParentChanged(EntityUid owner, RequiresGridComponent component, EntParentChangedMessage args)
    {
        if (args.OldParent == null)
            return;

        if (args.Transform.GridUid != null)
            return;

        if (TerminatingOrDeleted(owner))
            return;

        _destructible.DestroyEntity(owner);
    }
}
