// SPDX-FileCopyrightText: 2024 Aiden <aiden@djkraz.com>
// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Tadeo <td12233a@gmail.com>
// SPDX-FileCopyrightText: 2024 slarticodefast <161409025+slarticodefast@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using System.Diagnostics.CodeAnalysis;
using Content.Shared.Localizations;
using Content.Shared.Preferences;
using JetBrains.Annotations;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization;
using Robust.Shared.Utility;

namespace Content.Shared.Roles;

[UsedImplicitly]
[Serializable, NetSerializable]
public sealed partial class DepartmentTimeRequirement : JobRequirement
{
    /// <summary>
    /// Which department needs the required amount of time.
    /// </summary>
    [DataField(required: true)]
    public ProtoId<DepartmentPrototype> Department;

    /// <summary>
    /// How long (in seconds) this requirement is.
    /// </summary>
    [DataField(required: true)]
    public TimeSpan Time;

    public override bool Check(IEntityManager entManager,
        IPrototypeManager protoManager,
        HumanoidCharacterProfile? profile,
        IReadOnlyDictionary<string, TimeSpan> playTimes,
        [NotNullWhen(false)] out FormattedMessage? reason)
    {
        reason = new FormattedMessage();
        var playtime = TimeSpan.Zero;

        // Check all jobs' departments
        var department = protoManager.Index(Department);
        var jobs = department.Roles;
        string proto;

        // Check all jobs' playtime
        foreach (var other in jobs)
        {
            // The schema is stored on the Job role but we want to explode if the timer isn't found anyway.
            proto = protoManager.Index(other).PlayTimeTracker;

            playTimes.TryGetValue(proto, out var otherTime);
            playtime += otherTime;
        }

        var deptDiffSpan = Time - playtime;
        var deptDiff = deptDiffSpan.TotalMinutes;
        var formattedDeptDiff = ContentLocalizationManager.FormatPlaytime(deptDiffSpan);
        var nameDepartment = "role-timer-department-unknown";

        if (protoManager.TryIndex(Department, out var departmentIndexed))
        {
            nameDepartment = departmentIndexed.Name;
        }

        if (!Inverted)
        {
            if (deptDiff <= 0)
                return true;

            reason = FormattedMessage.FromMarkupPermissive(Loc.GetString(
                "role-timer-department-insufficient",
                ("time", formattedDeptDiff),
                ("department", Loc.GetString(nameDepartment)),
                ("departmentColor", department.Color.ToHex())));
            return false;
        }

        if (deptDiff <= 0)
        {
            reason = FormattedMessage.FromMarkupPermissive(Loc.GetString(
                "role-timer-department-too-high",
                ("time", formattedDeptDiff),
                ("department", Loc.GetString(nameDepartment)),
                ("departmentColor", department.Color.ToHex())));
            return false;
        }

        return true;
    }
}
