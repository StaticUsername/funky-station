// SPDX-FileCopyrightText: 2025 Skye <57879983+Rainbeon@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 kbarkevich <24629810+kbarkevich@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: AGPL-3.0-or-later AND MIT

using Robust.Client.Input;
using Content.Client.UserInterface.Controls;
using Content.Shared.Speech;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.CustomControls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Prototypes;

namespace Content.Client._Funkystation.BloodCult;

[GenerateTypedNameReferences]
public sealed partial class BloodCultCommuneWindow : FancyWindow
{
    public Action<string>? OnCommune;

    public BloodCultCommuneWindow()
    {
        RobustXamlLoader.Load(this);

        CommuneMessageSend.OnPressed += _ =>
        {
            OnCommune?.Invoke(CommuneMessage.Text);
        };

		CommuneMessage.OnTextEntered += _ =>
		{
			OnCommune?.Invoke(CommuneMessage.Text);
		};
    }

	protected override void Opened()
	{
		base.Opened();

		// Automatically highlight the commune text box for immediate typing convenience.
		CommuneMessage.GrabKeyboardFocus();
	}

    public void UpdateState(string name)
    {
        CommuneMessage.Text = name;
    }
}
