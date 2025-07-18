// SPDX-FileCopyrightText: 2023 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Movement.Components;
using Robust.Shared.Physics.Events;

namespace Content.Server.Movement.Systems;

public sealed class BoundarySystem : EntitySystem
{
    /*
     * The real reason this even exists is because with out mover controller it's really easy to clip out of bounds on chain shapes.
     */

    [Dependency] private readonly SharedTransformSystem _xform = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<BoundaryComponent, StartCollideEvent>(OnBoundaryCollide);
    }

    private void OnBoundaryCollide(Entity<BoundaryComponent> ent, ref StartCollideEvent args)
    {
        var center = _xform.GetWorldPosition(ent.Owner);
        var otherXform = Transform(args.OtherEntity);
        var collisionPoint = _xform.GetWorldPosition(otherXform);
        var offset = collisionPoint - center;
        offset = offset.Normalized() * (offset.Length() - ent.Comp.Offset);
        // If for whatever reason you want to yeet them to the other side.
        // offset = new Angle(MathF.PI).RotateVec(offset);

        _xform.SetWorldPosition((args.OtherEntity, otherXform), center + offset);
    }
}
