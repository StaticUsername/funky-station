// SPDX-FileCopyrightText: 2024 Luiz Costa <33888056+luizwritescode@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 themias <89101928+themias@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Humanoid;
using Content.Shared.Inventory;
using Robust.Shared.GameStates;

namespace Content.Shared.Clothing.Components;

[RegisterComponent, NetworkedComponent]
public sealed partial class FoldableClothingComponent : Component
{
    /// <summary>
    /// Which slots does this fit into when folded?
    /// </summary>
    [DataField]
    public SlotFlags? FoldedSlots;

    /// <summary>
    /// Which slots does this fit into when unfolded?
    /// </summary>
    [DataField]
    public SlotFlags? UnfoldedSlots;


    /// <summary>
    /// What equipped prefix does this have while in folded form?
    /// </summary>
    [DataField]
    public string? FoldedEquippedPrefix;

    /// <summary>
    /// What held prefix does this have while in folded form?
    /// </summary>
    [DataField]
    public string? FoldedHeldPrefix;

    /// <summary>
    /// Which layers does this hide when Unfolded? See <see cref="HumanoidVisualLayers"/> and <see cref="HideLayerClothingComponent"/>
    /// </summary>
    [DataField]
    public HashSet<HumanoidVisualLayers> UnfoldedHideLayers = new();

    /// <summary>
    /// Which layers does this hide when folded? See <see cref="HumanoidVisualLayers"/> and <see cref="HideLayerClothingComponent"/>
    /// </summary>
    [DataField]
    public HashSet<HumanoidVisualLayers> FoldedHideLayers = new();
}
