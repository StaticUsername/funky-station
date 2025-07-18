// SPDX-FileCopyrightText: 2024 TGRCDev <tgrc@tgrc.dev>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 Tay <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2025 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Shared.Pinpointer;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Map;

namespace Content.Client.Pinpointer.UI;

[GenerateTypedNameReferences]
public sealed partial class StationMapBeaconControl : Control, IComparable<StationMapBeaconControl>
{
    public readonly EntityCoordinates BeaconPosition;
    public Action<EntityCoordinates>? OnPressed;
    public string? Label => BeaconNameLabel.Text;
    private StyleBoxFlat _styleBox;
    public Color Color => _styleBox.BackgroundColor;

    public StationMapBeaconControl(EntityUid mapUid, SharedNavMapSystem.NavMapBeacon beacon)
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        BeaconPosition = new EntityCoordinates(mapUid, beacon.Position);

        _styleBox = new StyleBoxFlat { BackgroundColor = beacon.Color };
        ColorPanel.PanelOverride = _styleBox;
        BeaconNameLabel.Text = beacon.Text;

        MainButton.OnPressed += args => OnPressed?.Invoke(BeaconPosition);
    }

    public int CompareTo(StationMapBeaconControl? other)
    {
        if (other == null)
            return 1;

        // Group by color
        var colorCompare = Color.ToArgb().CompareTo(other.Color.ToArgb());
        if (colorCompare != 0)
        {
            return colorCompare;
        }

        // If same color, sort by text
        return string.Compare(Label, other.Label);
    }
}
