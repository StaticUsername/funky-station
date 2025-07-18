// SPDX-FileCopyrightText: 2022 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Andreas Kämper <andreas@kaemper.tech>
// SPDX-FileCopyrightText: 2022 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 Mervill <mervills.email@gmail.com>
// SPDX-FileCopyrightText: 2022 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.VendingMachines;
using Robust.Client.Animations;
using Robust.Client.GameObjects;

namespace Content.Client.VendingMachines;

public sealed class VendingMachineSystem : SharedVendingMachineSystem
{
    [Dependency] private readonly AnimationPlayerSystem _animationPlayer = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearanceSystem = default!;
    [Dependency] private readonly SharedUserInterfaceSystem _uiSystem = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<VendingMachineComponent, AppearanceChangeEvent>(OnAppearanceChange);
        SubscribeLocalEvent<VendingMachineComponent, AnimationCompletedEvent>(OnAnimationCompleted);
        SubscribeLocalEvent<VendingMachineComponent, AfterAutoHandleStateEvent>(OnVendingAfterState);
    }

    private void OnVendingAfterState(EntityUid uid, VendingMachineComponent component, ref AfterAutoHandleStateEvent args)
    {
        if (_uiSystem.TryGetOpenUi<VendingMachineBoundUserInterface>(uid, VendingMachineUiKey.Key, out var bui))
        {
            bui.Refresh();
        }
    }

    private void OnAnimationCompleted(EntityUid uid, VendingMachineComponent component, AnimationCompletedEvent args)
    {
        if (!TryComp<SpriteComponent>(uid, out var sprite))
            return;

        if (!TryComp<AppearanceComponent>(uid, out var appearance) ||
            !_appearanceSystem.TryGetData<VendingMachineVisualState>(uid, VendingMachineVisuals.VisualState, out var visualState, appearance))
        {
            visualState = VendingMachineVisualState.Normal;
        }

        UpdateAppearance(uid, visualState, component, sprite);
    }

    private void OnAppearanceChange(EntityUid uid, VendingMachineComponent component, ref AppearanceChangeEvent args)
    {
        if (args.Sprite == null)
            return;

        if (!args.AppearanceData.TryGetValue(VendingMachineVisuals.VisualState, out var visualStateObject) ||
            visualStateObject is not VendingMachineVisualState visualState)
        {
            visualState = VendingMachineVisualState.Normal;
        }

        UpdateAppearance(uid, visualState, component, args.Sprite);
    }

    private void UpdateAppearance(EntityUid uid, VendingMachineVisualState visualState, VendingMachineComponent component, SpriteComponent sprite)
    {
        SetLayerState(VendingMachineVisualLayers.Base, component.OffState, sprite);

        switch (visualState)
        {
            case VendingMachineVisualState.Normal:
                SetLayerState(VendingMachineVisualLayers.BaseUnshaded, component.NormalState, sprite);
                SetLayerState(VendingMachineVisualLayers.Screen, component.ScreenState, sprite);
                break;

            case VendingMachineVisualState.Deny:
                if (component.LoopDenyAnimation)
                    SetLayerState(VendingMachineVisualLayers.BaseUnshaded, component.DenyState, sprite);
                else
                    PlayAnimation(uid, VendingMachineVisualLayers.BaseUnshaded, component.DenyState, component.DenyDelay, sprite);

                SetLayerState(VendingMachineVisualLayers.Screen, component.ScreenState, sprite);
                break;

            case VendingMachineVisualState.Eject:
                PlayAnimation(uid, VendingMachineVisualLayers.BaseUnshaded, component.EjectState, component.EjectDelay, sprite);
                SetLayerState(VendingMachineVisualLayers.Screen, component.ScreenState, sprite);
                break;

            case VendingMachineVisualState.Broken:
                HideLayers(sprite);
                SetLayerState(VendingMachineVisualLayers.Base, component.BrokenState, sprite);
                break;

            case VendingMachineVisualState.Off:
                HideLayers(sprite);
                break;
        }
    }

    private static void SetLayerState(VendingMachineVisualLayers layer, string? state, SpriteComponent sprite)
    {
        if (string.IsNullOrEmpty(state))
            return;

        sprite.LayerSetVisible(layer, true);
        sprite.LayerSetAutoAnimated(layer, true);
        sprite.LayerSetState(layer, state);
    }

    private void PlayAnimation(EntityUid uid, VendingMachineVisualLayers layer, string? state, float animationTime, SpriteComponent sprite)
    {
        if (string.IsNullOrEmpty(state))
            return;

        if (!_animationPlayer.HasRunningAnimation(uid, state))
        {
            var animation = GetAnimation(layer, state, animationTime);
            sprite.LayerSetVisible(layer, true);
            _animationPlayer.Play(uid, animation, state);
        }
    }

    private static Animation GetAnimation(VendingMachineVisualLayers layer, string state, float animationTime)
    {
        return new Animation
        {
            Length = TimeSpan.FromSeconds(animationTime),
            AnimationTracks =
                {
                    new AnimationTrackSpriteFlick
                    {
                        LayerKey = layer,
                        KeyFrames =
                        {
                            new AnimationTrackSpriteFlick.KeyFrame(state, 0f)
                        }
                    }
                }
        };
    }

    private static void HideLayers(SpriteComponent sprite)
    {
        HideLayer(VendingMachineVisualLayers.BaseUnshaded, sprite);
        HideLayer(VendingMachineVisualLayers.Screen, sprite);
    }

    private static void HideLayer(VendingMachineVisualLayers layer, SpriteComponent sprite)
    {
        if (!sprite.LayerMapTryGet(layer, out var actualLayer))
            return;

        sprite.LayerSetVisible(actualLayer, false);
    }
}
