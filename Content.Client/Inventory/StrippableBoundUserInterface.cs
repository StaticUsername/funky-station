// SPDX-FileCopyrightText: 2020 Exp <theexp111@gmail.com>
// SPDX-FileCopyrightText: 2020 Víctor Aguilera Puerto <6766154+Zumorica@users.noreply.github.com>
// SPDX-FileCopyrightText: 2020 nuke <47336974+nuke-makes-games@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Acruid <shatter66@gmail.com>
// SPDX-FileCopyrightText: 2021 Galactic Chimp <63882831+GalacticChimp@users.noreply.github.com>
// SPDX-FileCopyrightText: 2021 Paul <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2021 Paul Ritter <ritter.paul1@googlemail.com>
// SPDX-FileCopyrightText: 2021 Vera Aguilera Puerto <gradientvera@outlook.com>
// SPDX-FileCopyrightText: 2022 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 keronshb <54602815+keronshb@users.noreply.github.com>
// SPDX-FileCopyrightText: 2022 mirrorcult <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2022 wrexbe <81056464+wrexbe@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2024 AJCM-git <60196617+AJCM-git@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Aidenkrz <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Leon Friedrich <60421075+ElectroJr@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 PJBot <pieterjan.briers+bot@gmail.com>
// SPDX-FileCopyrightText: 2024 Pieter-Jan Briers <pieterjan.briers+git@gmail.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 RadsammyT <32146976+RadsammyT@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 corresp0nd <46357632+corresp0nd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Linq;
using System.Numerics;
using Content.Client.Examine;
using Content.Client.Strip;
using Content.Client.Stylesheets;
using Content.Client.UserInterface.Controls;
using Content.Client.UserInterface.Systems.Hands.Controls;
using Content.Client.Verbs.UI;
using Content.Shared._EstacaoPirata.Cards.Card;
using Content.Shared._EstacaoPirata.Cards.Hand;
using Content.Shared.Cuffs;
using Content.Shared.Cuffs.Components;
using Content.Shared.Ensnaring.Components;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Input;
using Content.Shared.Inventory;
using Content.Shared.Inventory.VirtualItem;
using Content.Shared.Strip.Components;
using JetBrains.Annotations;
using Robust.Client.GameObjects;
using Robust.Client.Player;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Input;
using Robust.Shared.Map;
using static Content.Client.Inventory.ClientInventorySystem;
using static Robust.Client.UserInterface.Control;

namespace Content.Client.Inventory
{
    [UsedImplicitly]
    public sealed class StrippableBoundUserInterface : BoundUserInterface
    {
        [Dependency] private readonly IPlayerManager _player = default!;
        [Dependency] private readonly IUserInterfaceManager _ui = default!;

        private readonly ExamineSystem _examine;
        private readonly InventorySystem _inv;
        private readonly SharedCuffableSystem _cuffable;
        private readonly StrippableSystem _strippable;

        [ViewVariables]
        private const int ButtonSeparation = 4;

        [ViewVariables]
        public const string HiddenPocketEntityId = "StrippingHiddenEntity";

        [ViewVariables]
        private StrippingMenu? _strippingMenu;

        [ViewVariables]
        private readonly EntityUid _virtualHiddenEntity;

        public StrippableBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
        {
            _examine = EntMan.System<ExamineSystem>();
            _inv = EntMan.System<InventorySystem>();
            _cuffable = EntMan.System<SharedCuffableSystem>();
            _strippable = EntMan.System<StrippableSystem>();

            _virtualHiddenEntity = EntMan.SpawnEntity(HiddenPocketEntityId, MapCoordinates.Nullspace);
        }

        protected override void Open()
        {
            base.Open();

            _strippingMenu = this.CreateWindowCenteredLeft<StrippingMenu>();
            _strippingMenu.OnDirty += UpdateMenu;
            _strippingMenu.Title = Loc.GetString("strippable-bound-user-interface-stripping-menu-title", ("ownerName", Identity.Name(Owner, EntMan)));
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            if (_strippingMenu != null)
                _strippingMenu.OnDirty -= UpdateMenu;

            EntMan.DeleteEntity(_virtualHiddenEntity);
            base.Dispose(disposing);
        }

        public void DirtyMenu()
        {
            if (_strippingMenu != null)
                _strippingMenu.Dirty = true;
        }

        public void UpdateMenu()
        {
            if (_strippingMenu == null)
                return;

            _strippingMenu.ClearButtons();

            if (EntMan.TryGetComponent<InventoryComponent>(Owner, out var inv))
            {
                foreach (var slot in inv.Slots)
                {
                    AddInventoryButton(Owner, slot.Name, inv);
                }
            }

            if (EntMan.TryGetComponent<HandsComponent>(Owner, out var handsComp) && handsComp.CanBeStripped)
            {
                // good ol hands shit code. there is a GuiHands comparer that does the same thing... but these are hands
                // and not gui hands... which are different...
                foreach (var hand in handsComp.Hands.Values)
                {
                    if (hand.Location != HandLocation.Right)
                        continue;

                    AddHandButton(hand);
                }

                foreach (var hand in handsComp.Hands.Values)
                {
                    if (hand.Location != HandLocation.Middle)
                        continue;

                    AddHandButton(hand);
                }

                foreach (var hand in handsComp.Hands.Values)
                {
                    if (hand.Location != HandLocation.Left)
                        continue;

                    AddHandButton(hand);
                }
            }

            // snare-removal button. This is just the old button before the change to item slots. It is pretty out of place.
            if (EntMan.TryGetComponent<EnsnareableComponent>(Owner, out var snare) && snare.IsEnsnared)
            {
                var button = new Button()
                {
                    Text = Loc.GetString("strippable-bound-user-interface-stripping-menu-ensnare-button"),
                    StyleClasses = { StyleBase.ButtonOpenRight }
                };

                button.OnPressed += (_) => SendPredictedMessage(new StrippingEnsnareButtonPressed());

                _strippingMenu.SnareContainer.AddChild(button);
            }

            // TODO fix layout container measuring (its broken atm).
            // _strippingMenu.InvalidateMeasure();
            // _strippingMenu.Contents.Measure(Vector2Helpers.Infinity);

            // TODO allow windows to resize based on content's desired size

            // for now: shit-code
            // this breaks for drones (too many hands, lots of empty vertical space), and looks shit for monkeys and the like.
            // but the window is realizable, so eh.
            _strippingMenu.SetSize = new Vector2(220, snare?.IsEnsnared == true ? 550 : 530);
        }

        private void AddHandButton(Hand hand)
        {
            var button = new HandButton(hand.Name, hand.Location);

            button.Pressed += SlotPressed;

            if (EntMan.TryGetComponent<VirtualItemComponent>(hand.HeldEntity, out var virt))
            {
                button.Blocked = true;
                if (EntMan.TryGetComponent<CuffableComponent>(Owner, out var cuff) && _cuffable.GetAllCuffs(cuff).Contains(virt.BlockingEntity))
                    button.BlockedRect.MouseFilter = MouseFilterMode.Ignore;
            }
            //Goobstation: Cards are always hidden. NO CHEATING FOR U.
            var isCard = EntMan.HasComponent<CardComponent>(hand.HeldEntity) ||
                         EntMan.HasComponent<CardHandComponent>(hand.HeldEntity);
            UpdateEntityIcon(button, isCard ? _virtualHiddenEntity : hand.HeldEntity);
            _strippingMenu!.HandsContainer.AddChild(button);
        }

        private void SlotPressed(GUIBoundKeyEventArgs ev, SlotControl slot)
        {
            // TODO: allow other interactions? Verbs? But they should then generate a pop-up and/or have a delay so the
            // user that is being stripped can prevent the verbs from being exectuted.
            // So for now: only stripping & examining
            if (ev.Function == EngineKeyFunctions.Use)
            {
                SendPredictedMessage(new StrippingSlotButtonPressed(slot.SlotName, slot is HandButton));
                return;
            }

            if (slot.Entity == null)
                return;

            if (ev.Function == ContentKeyFunctions.ExamineEntity)
            {
                _examine.DoExamine(slot.Entity.Value);
                ev.Handle();
            }
            else if (ev.Function == EngineKeyFunctions.UseSecondary)
            {
                _ui.GetUIController<VerbMenuUIController>().OpenVerbMenu(slot.Entity.Value);
                ev.Handle();
            }
        }

        private void AddInventoryButton(EntityUid invUid, string slotId, InventoryComponent inv)
        {
            if (!_inv.TryGetSlotContainer(invUid, slotId, out var container, out var slotDef, inv))
                return;

            var entity = container.ContainedEntity;

            // If this is a full pocket, obscure the real entity
            // this does not work for modified clients because they are still sent the real entity
            if (entity != null && _strippable.IsStripHidden(slotDef, _player.LocalEntity))
                entity = _virtualHiddenEntity;

            // Goobstation: Playing Cards are always obscured in strip menu.
            // I wanted to make the cards themselves appear hidden but this is simpler
            var isCard = EntMan.HasComponent<CardComponent>(entity) ||
                         EntMan.HasComponent<CardHandComponent>(entity);
            if (entity != null && isCard)
            {
                entity = _virtualHiddenEntity;
            }

            var button = new SlotButton(new SlotData(slotDef, container));
            button.Pressed += SlotPressed;

            _strippingMenu!.InventoryContainer.AddChild(button);

            UpdateEntityIcon(button, entity);

            LayoutContainer.SetPosition(button, slotDef.StrippingWindowPos * (SlotControl.DefaultButtonSize + ButtonSeparation));
        }

        private void UpdateEntityIcon(SlotControl button, EntityUid? entity)
        {
            // Hovering, highlighting & storage are features of general hands & inv GUIs. This UI just re-uses these because I'm lazy.
            button.ClearHover();
            button.StorageButton.Visible = false;

            if (entity == null)
            {
                button.SetEntity(null);
                return;
            }

            EntityUid? viewEnt;
            if (EntMan.TryGetComponent<VirtualItemComponent>(entity, out var virt))
                viewEnt = EntMan.HasComponent<SpriteComponent>(virt.BlockingEntity) ? virt.BlockingEntity : null;
            else if (EntMan.HasComponent<SpriteComponent>(entity))
                viewEnt = entity;
            else
                return;

            button.SetEntity(viewEnt);
        }
    }
}
