// SPDX-FileCopyrightText: 2021 Galactic Chimp <63882831+GalacticChimp@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Galactic Chimp <GalacticChimpanzee@gmail.com>
// SPDX-FileCopyrightText: 2022 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2022 Alex Evgrashin <aevgrashin@yandex.ru>
// SPDX-FileCopyrightText: 2022 Morb <14136326+Morb0@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Rane <60792108+Elijahrane@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2023 Pieter-Jan Briers <pieterjan.briers@gmail.com>
// SPDX-FileCopyrightText: 2023 Vyacheslav Kovalevsky <40753025+Slava0135@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 metalgearsloth <comedian_vs_clown@hotmail.com>
// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 GreaseMonk <1354802+GreaseMonk@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 MilenVolf <63782763+MilenVolf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Piras314 <p1r4s@proton.me>
// SPDX-FileCopyrightText: 2024 Plykiya <58439124+Plykiya@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 Tayrtahn <tayrtahn@gmail.com>
// SPDX-FileCopyrightText: 2024 blueDev2 <89804215+blueDev2@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 pa.pecherskij <pa.pecherskij@interfax.ru>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Audio;
using Content.Shared.Hands;
using Content.Shared.Interaction;
using Content.Shared.Interaction.Events;
using Content.Shared.Maps;
using Content.Shared.Mobs;
using Content.Shared.Popups;
using Content.Shared.Sound.Components;
using Content.Shared.Throwing;
using Content.Shared.UserInterface;
using Content.Shared.Whitelist;
using JetBrains.Annotations;
using Robust.Shared.Audio;
using Robust.Shared.Audio.Systems;
using Robust.Shared.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Map.Components;
using Robust.Shared.Network;
using Robust.Shared.Physics.Components;
using Robust.Shared.Physics.Events;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Shared.Sound;

/// <summary>
/// Will play a sound on various events if the affected entity has a component derived from BaseEmitSoundComponent
/// </summary>
[UsedImplicitly]
public abstract class SharedEmitSoundSystem : EntitySystem
{
    [Dependency] protected readonly IGameTiming Timing = default!;
    [Dependency] private readonly INetManager _netMan = default!;
    [Dependency] private readonly ITileDefinitionManager _tileDefMan = default!;
    [Dependency] protected readonly IRobustRandom Random = default!;
    [Dependency] private   readonly SharedAmbientSoundSystem _ambient = default!;
    [Dependency] private   readonly SharedAudioSystem _audioSystem = default!;
    [Dependency] protected readonly SharedPopupSystem Popup = default!;
    [Dependency] private readonly SharedMapSystem _map = default!;
    [Dependency] private readonly EntityWhitelistSystem _whitelistSystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<EmitSoundOnSpawnComponent, MapInitEvent>(OnEmitSpawnOnInit);
        SubscribeLocalEvent<EmitSoundOnLandComponent, LandEvent>(OnEmitSoundOnLand);
        SubscribeLocalEvent<EmitSoundOnUseComponent, UseInHandEvent>(OnEmitSoundOnUseInHand);
        SubscribeLocalEvent<EmitSoundOnThrowComponent, ThrownEvent>(OnEmitSoundOnThrown);
        SubscribeLocalEvent<EmitSoundOnActivateComponent, ActivateInWorldEvent>(OnEmitSoundOnActivateInWorld);
        SubscribeLocalEvent<EmitSoundOnPickupComponent, GotEquippedHandEvent>(OnEmitSoundOnPickup);
        SubscribeLocalEvent<EmitSoundOnDropComponent, DroppedEvent>(OnEmitSoundOnDrop);
        SubscribeLocalEvent<EmitSoundOnInteractUsingComponent, InteractUsingEvent>(OnEmitSoundOnInteractUsing);
        SubscribeLocalEvent<EmitSoundOnUIOpenComponent, AfterActivatableUIOpenEvent>(HandleEmitSoundOnUIOpen);

        SubscribeLocalEvent<EmitSoundOnCollideComponent, StartCollideEvent>(OnEmitSoundOnCollide);

        SubscribeLocalEvent<SoundWhileAliveComponent, MobStateChangedEvent>(OnMobState);

        // We need to handle state manually here
        // BaseEmitSoundComponent isn't registered so we have to subscribe to each one
        // TODO: Make it use autonetworking instead of relying on inheritance
        SubscribeEmitComponent<EmitSoundOnActivateComponent>();
        SubscribeEmitComponent<EmitSoundOnCollideComponent>();
        SubscribeEmitComponent<EmitSoundOnDropComponent>();
        SubscribeEmitComponent<EmitSoundOnInteractUsingComponent>();
        SubscribeEmitComponent<EmitSoundOnLandComponent>();
        SubscribeEmitComponent<EmitSoundOnPickupComponent>();
        SubscribeEmitComponent<EmitSoundOnSpawnComponent>();
        SubscribeEmitComponent<EmitSoundOnThrowComponent>();
        SubscribeEmitComponent<EmitSoundOnUIOpenComponent>();
        SubscribeEmitComponent<EmitSoundOnUseComponent>();

        // Helper method so it's a little less ugly
        void SubscribeEmitComponent<T>() where T : BaseEmitSoundComponent
        {
            SubscribeLocalEvent<T, ComponentGetState>(GetBaseEmitState);
            SubscribeLocalEvent<T, ComponentHandleState>(HandleBaseEmitState);
        }
    }

    private static void GetBaseEmitState<T>(Entity<T> ent, ref ComponentGetState args) where T : BaseEmitSoundComponent
    {
        args.State = new EmitSoundComponentState(ent.Comp.Sound);
    }

    private static void HandleBaseEmitState<T>(Entity<T> ent, ref ComponentHandleState args) where T : BaseEmitSoundComponent
    {
        if (args.Current is not EmitSoundComponentState state)
            return;

        ent.Comp.Sound = state.Sound switch
        {
            SoundPathSpecifier pathSpec => new SoundPathSpecifier(pathSpec.Path, pathSpec.Params),
            SoundCollectionSpecifier collectionSpec => collectionSpec.Collection != null
                ? new SoundCollectionSpecifier(collectionSpec.Collection, collectionSpec.Params)
                : null,
            _ => null,
        };
    }

    private void HandleEmitSoundOnUIOpen(EntityUid uid, EmitSoundOnUIOpenComponent component, AfterActivatableUIOpenEvent args)
    {
        if (_whitelistSystem.IsBlacklistFail(component.Blacklist, args.User))
        {
            TryEmitSound(uid, component, args.User);
        }
    }

    private void OnMobState(Entity<SoundWhileAliveComponent> entity, ref MobStateChangedEvent args)
    {
        // Disable this component rather than removing it because it can be brought back to life.
        if (TryComp<SpamEmitSoundComponent>(entity, out var comp))
        {
            comp.Enabled = args.NewMobState == MobState.Alive;
            Dirty(entity.Owner, comp);
        }

        _ambient.SetAmbience(entity.Owner, args.NewMobState != MobState.Dead);
    }

    private void OnEmitSpawnOnInit(EntityUid uid, EmitSoundOnSpawnComponent component, MapInitEvent args)
    {
        TryEmitSound(uid, component, predict: false);
    }

    private void OnEmitSoundOnLand(EntityUid uid, BaseEmitSoundComponent component, ref LandEvent args)
    {
        if (!args.PlaySound ||
            !TryComp(uid, out TransformComponent? xform) ||
            !TryComp<MapGridComponent>(xform.GridUid, out var grid))
        {
            return;
        }

        var tile = _map.GetTileRef(xform.GridUid.Value, grid, xform.Coordinates);

        // Handle maps being grids (we'll still emit the sound).
        if (xform.GridUid != xform.MapUid && tile.IsSpace(_tileDefMan))
            return;

        // hand throwing not predicted sadly
        TryEmitSound(uid, component, args.User, false);
    }

    private void OnEmitSoundOnUseInHand(EntityUid uid, EmitSoundOnUseComponent component, UseInHandEvent args)
    {
        // Intentionally not checking whether the interaction has already been handled.
        TryEmitSound(uid, component, args.User);

        if (component.Handle)
            args.Handled = true;
    }

    private void OnEmitSoundOnThrown(EntityUid uid, BaseEmitSoundComponent component, ref ThrownEvent args)
    {
        TryEmitSound(uid, component, args.User, false);
    }

    private void OnEmitSoundOnActivateInWorld(EntityUid uid, EmitSoundOnActivateComponent component, ActivateInWorldEvent args)
    {
        // Intentionally not checking whether the interaction has already been handled.
        TryEmitSound(uid, component, args.User);

        if (component.Handle)
            args.Handled = true;
    }

    private void OnEmitSoundOnPickup(EntityUid uid, EmitSoundOnPickupComponent component, GotEquippedHandEvent args)
    {
        TryEmitSound(uid, component, args.User);
    }

    private void OnEmitSoundOnDrop(EntityUid uid, EmitSoundOnDropComponent component, DroppedEvent args)
    {
        TryEmitSound(uid, component, args.User);
    }

    private void OnEmitSoundOnInteractUsing(Entity<EmitSoundOnInteractUsingComponent> ent, ref InteractUsingEvent args)
    {
        if (_whitelistSystem.IsWhitelistPass(ent.Comp.Whitelist, args.Used))
        {
            TryEmitSound(ent, ent.Comp, args.User);
        }
    }
    protected void TryEmitSound(EntityUid uid, BaseEmitSoundComponent component, EntityUid? user=null, bool predict=true)
    {
        if (component.Sound == null)
            return;

        if (component.Positional)
        {
            var coords = Transform(uid).Coordinates;
            if (predict)
                _audioSystem.PlayPredicted(component.Sound, coords, user);
            else if (_netMan.IsServer)
                // don't predict sounds that client couldn't have played already
                _audioSystem.PlayPvs(component.Sound, coords);
        }
        else
        {
            if (predict)
                _audioSystem.PlayPredicted(component.Sound, uid, user);
            else if (_netMan.IsServer)
                // don't predict sounds that client couldn't have played already
                _audioSystem.PlayPvs(component.Sound, uid);
        }
    }

    private void OnEmitSoundOnCollide(EntityUid uid, EmitSoundOnCollideComponent component, ref StartCollideEvent args)
    {
        if (!args.OurFixture.Hard ||
            !args.OtherFixture.Hard ||
            !TryComp<PhysicsComponent>(uid, out var physics) ||
            physics.LinearVelocity.Length() < component.MinimumVelocity ||
            Timing.CurTime < component.NextSound ||
            MetaData(uid).EntityPaused)
        {
            return;
        }

        const float MaxVolumeVelocity = 10f;
        const float MinVolume = -10f;
        const float MaxVolume = 2f;

        var fraction = MathF.Min(1f, (physics.LinearVelocity.Length() - component.MinimumVelocity) / MaxVolumeVelocity);
        var volume = MinVolume + (MaxVolume - MinVolume) * fraction;
        component.NextSound = Timing.CurTime + EmitSoundOnCollideComponent.CollideCooldown;
        var sound = component.Sound;

        if (_netMan.IsServer && sound != null)
        {
            _audioSystem.PlayPvs(_audioSystem.ResolveSound(sound), uid, AudioParams.Default.WithVolume(volume));
        }
    }

    public virtual void SetEnabled(Entity<SpamEmitSoundComponent?> entity, bool enabled)
    {
    }
}
