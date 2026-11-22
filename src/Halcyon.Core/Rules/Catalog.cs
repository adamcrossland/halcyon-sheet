using Halcyon.Core.Model;

namespace Halcyon.Core.Rules;

/// <summary>
/// Rules content from "Classes and Peoples", "Weapons and Gear", "The Book of Odds"
/// and the Grit System SRD. Summaries are short enough to fit on the sheet.
/// </summary>
public static class Catalog
{
    public static readonly int[][] ScoreArrays =
    [
        [12, 10, 8, 6, 6],
        [12, 10, 8, 8, 4],
    ];

    public const int PowersAtFirstLevel = 4;
    public const int PeopleChoices = 2;
    public const int PoolPerLevel = 6;
    public const int MaxNaturalDr = 4;

    // ---------------------------------------------------------------- Weird (Spill-touch)

    public static readonly PowerDef[] WeirdPowers =
    [
        new("aetherspeak", "Aetherspeak", "standard", "X GRT (min 2)", "Link X minds telepathically for level + X minutes, regardless of language.") { SrdSource = "SRD Weird" },
        new("alluring", "Alluring", "standard", "—", "While you flirt or tease, others must pass WIL/resist other vs 5 + level to act.") { SrdSource = "SRD Weird" },
        new("arcanofusil", "Arcanofusil", "standard", "2 GRT (+4)", "+2 to hit, d6 damage needle hail; on a hit, burn 4 more for +d6.") { SrdSource = "SRD Weird" },
        new("augur", "Augur", "10-minute", "10−dREA GRT, 2 HEA", "Ask one short question; at round's end the nameless things answer truly.") { SrdSource = "SRD Weird" },
        new("body-of-wind", "Body of wind", "free", "4 GRT", "For level rounds, physical attacks only harm you on an attack check of 9+.") { SrdSource = "SRD Weird" },
        new("cloak", "Cloak", "standard", "5 GRT", "Invisible to non-Weird detection for 5 + level minutes; no other Weird meanwhile.") { SrdSource = "SRD Weird" },
        new("curse", "Curse", "standard", "8 GRT", "Withering (−8 HEA) or draining (−6 GRT) each time the target rolls a 1, dWIL times.") { SrdSource = "SRD Weird" },
        new("dance", "Dance", "standard", "6 GRT", "A weapon fights on its own until combat ends (MOV d20, HEA level + 2, your COM).") { SrdSource = "SRD Weird" },
        new("defy-death", "Defy death", "passive", "—", "10 min after death, return with dWIL HEA; MOV or WIL drops a die type each time.") { SrdSource = "SRD Weird" },
        new("glisten", "Glisten", "standard", "10 GRT", "COM touch attack for 6d6; on a miss, you and the target both take 3d6.") { SrdSource = "SRD Weird" },
        new("puppet", "Puppet", "standard", "2d8 GRT", "Control one target for level hours while you concentrate; self-harm allows WIL vs CHA.") { SrdSource = "SRD Weird" },
        new("reanimate", "Reanimate", "standard", "12X GRT", "Bring one person back from the dead for X hours. They don't want to be.") { SrdSource = "SRD Weird" },
        new("slip", "Slip", "standard", "3/6/9 GRT", "Teleport to a point you can see: near, far, or beyond range.") { SrdSource = "SRD Weird" },
        new("skinwalk", "Skinwalk", "2-round", "2d6 GRT", "Become your chosen beast form (fly, swim, tiny and fast, or huge) or change back.") { SrdSource = "SRD Weird" },
        new("spawn-hedgeward", "Spawn hedgeward", "standard", "3/6/9 GRT", "A wall of thorns: DR 1/2/3 while you stay put, d4/d6/d8 to foes close/near/far.") { SrdSource = "SRD Weird" },
        new("summon-the-horde", "Summon the horde", "standard", "5 GRT", "Vermin absorb 3 damage/round for dCHA + level rounds; close attackers lose level HEA.") { SrdSource = "SRD Weird" },
        new("terrify", "Terrify", "standard", "8 GRT (6+X)", "One person flees in terror for level hours (or X people for level + X rounds).") { SrdSource = "SRD Weird" },
        new("touch-dream", "Touch dream", "standard", "2 GRT", "REA: pull a hand-sized object (≤ level × 10 scrip) from thin air; fail and something bad comes.") { SrdSource = "SRD Weird" },
        new("trade-eyes", "Trade eyes", "standard", "d20 GRT", "Swap minds with someone for up to level days; WIL check or lose d10 HEA.") { SrdSource = "SRD Weird" },
        new("weirdlock", "Weirdlock", "passive", "—", "Once per session, on a natural 1, you shoot somebody. No roll to hit.") { SrdSource = "SRD Weird" },
    ];

    /// <summary>Weird powers the Sensitive's Spill-touched power recommends.</summary>
    public static readonly string[] RecommendedSpillTouch =
        ["slip", "body-of-wind", "cloak", "augur", "glisten", "touch-dream", "terrify", "curse", "defy-death"];

    // ---------------------------------------------------------------- Advanced powers

    public static readonly PowerDef[] AdvancedPowers =
    [
        new("become-ronin", "Become ronin", "passive", "—", "Dirty work pays you 200% without haggling. Your own bosses fear you.") { SrdSource = "SRD advanced" },
        new("chain-attack", "Chain attack", "free", "2 GRT", "After you take down a target, attack a new foe in close range this turn.") { SrdSource = "SRD advanced" },
        new("disarm", "Disarm", "standard", "5 GRT", "Disarm one target in close range. Only Weird powers resist.") { SrdSource = "SRD advanced" },
        new("explode", "Explode", "free", "1 GRT each", "When a damage die rolls max, burn 1 GRT to roll it again and add.") { SrdSource = "SRD advanced" },
        new("enshroud", "Enshroud", "passive", "1 GRT", "+3 on MOV/sneak checks (always on). Can be taken more than once.") { SrdSource = "SRD advanced" },
        new("fence", "Fence", "free", "—", "Hide and move a small object (≤5 kg) undetectable except by Weird means.") { SrdSource = "SRD advanced" },
        new("fight", "Fight", "passive", "—", "Unarmed damage is dCOM instead of d2; use dCOM for any melee weapon.") { SrdSource = "SRD (Killer)" },
        new("lighten-the-mood", "Lighten the mood", "passive", "—", "Gain dCHA GRT whenever your in-character joke makes a player laugh.") { SrdSource = "SRD advanced" },
        new("make-whole", "Make whole", "free", "3 GRT", "Gain 1 HEA.") { SrdSource = "SRD advanced" },
        new("maximize", "Maximize", "free", "2 GRT", "Burn before you attack: on a hit, deal maximum damage.") { SrdSource = "SRD advanced" },
        new("parasitize", "Parasitize", "standard", "—", "Touch a sapient: they lose GRT equal to your level, and you gain it.") { SrdSource = "SRD advanced" },
        new("scratch", "Scratch", "free", "Xd3 GRT", "For X rounds, claws deal d6 + X and ignore DR X.") { SrdSource = "SRD advanced" },
        new("wild-boost", "Wild boost", "free", "varies", "Any roll but a 1 succeeds; burn GRT equal to the shortfall (3 HEA per missing GRT).") { SrdSource = "SRD advanced" },
    ];

    // ---------------------------------------------------------------- Classes

    public static readonly ClassDef[] Classes =
    [
        new("guide", "Guide", "Navigation, getting everyone in and out",
            "You walk point. You read the Spill the way an old sailor reads weather.",
            "The itch", "If you finish an adventure without going into the Spill, make all WIL checks with disadvantage until you go back in.",
            [
                new("step-where-i-step", "Step where I step", "free", "2 GRT/follower", "Lead the line: you roll once for every snag-avoid check and everyone uses your result. Fail and all suffer it.") { Marquee = true },
                new("throw-a-bolt", "Throw a bolt", "standard", "1 GRT", "GM says if a snag lies on your path within near range, and roughly where. WIL/perceive 9+: what kind.") ,
                new("read-the-ground", "Read the ground", "10-minute", "3/5/7 GRT", "Name a Spill destination, roll REA/investigate: a route past one major hazard, no random encounter."),
                new("gut-feeling", "Gut feeling", "passive", "—", "The Spill never ambushes you: act in its ambush round and warn one ally, who also acts."),
                new("the-long-way-round", "The long way round", "free", "5 GRT +1/person", "When lost or pursued, WIL 7+: lead everyone back to the last safe place. Fail: only you, 2d6 damage."),
                new("sleep-rough", "Sleep rough", "passive", "—", "Camp in the Spill: no random encounters, +level extra pool for all, no exposure for that night."),
                new("track", "Track", "10-minute", "3/5/7 GRT", "Follow or locate one entity (easy/difficult/near-impossible), then roll REA. Fail: something else interesting.") { SrdSource = "SRD (Killer)" },
            ],
            [
                new("Old hand", 6, 10, 8, 12, 6, ["step-where-i-step", "throw-a-bolt", "read-the-ground", "gut-feeling"]),
                new("Tout", 6, 10, 6, 12, 8, ["step-where-i-step", "throw-a-bolt", "the-long-way-round", "sleep-rough"], "Guides paying tourists"),
            ],
            ["bag-of-bolts", "gas-mask", "hand-crank-torch", "map-common", "heavy-long-coat"],
            []),

        new("gun", "Gun", "Protection, violence, intimidation",
            "Ex-Cordon, ex-cop, ex-bodyguard. On a run you're the reason the crew comes home.",
            "The record", "The Cordon knows your face. When the authorities want someone to blame for violence in your part of the city, they come for you first.",
            [
                new("fan-the-hammer", "Fan the hammer", "standard", "3X GRT (X≤5)", "Revolver in hand: make X+1 attacks this round, split among targets. Cylinder empties; reload is standard.") { Marquee = true },
                new("quickdraw", "Quickdraw", "free", "4 GRT", "When initiative begins, act first, before ambushes. Your first attack this combat has advantage."),
                new("cover-me", "Cover me", "standard", "3 GRT", "Until your next turn, enemies in far range attack at disadvantage or take cover. Close allies +1 MOV/dodge."),
                new("take-the-bullet", "Take the bullet", "free", "3 GRT", "When an ally in close range is hit, take the hit instead and reduce the damage by your level (after armor)."),
                new("hot-cell", "Hot cell", "passive", "2 GRT", "Your charged weapons never misfire; burn 2 GRT for one last shot when dry. Know any weapon at a glance."),
                new("reputation", "Reputation", "passive", "—", "When you hit someone, their side tests conviction (3 or lower). Visibly armed: CHA/intimidate with advantage."),
                new("counterattack", "Counterattack", "passive", "—", "When a close-range attacker rolls a 1, you automatically strike them (fists: dCOM). No attack check.") { SrdSource = "SRD (Killer)" },
            ],
            [
                new("Cordon deserter", 12, 10, 6, 8, 6, ["fan-the-hammer", "quickdraw", "cover-me", "hot-cell"]),
                new("Bodyguard", 12, 8, 6, 10, 6, ["fan-the-hammer", "take-the-bullet", "cover-me", "counterattack"]),
            ],
            [],
            [
                new("Sidearm", [new("Service revolver", ["service-revolver"]), new("Autopistol", ["autopistol"])]),
                new("Protection", [new("Ballistic vest", ["ballistic-vest"]), new("Stinger with one cell", ["stinger"])]),
            ]),

        new("fixer", "Fixer", "Money, contacts, papers, leverage",
            "You know who buys, who sells, and which Cordon sergeant drinks at which bar.",
            "The debt", "You owe someone dangerous. At the start of each adventure, pay them 20 × your level scrip, or do the \"favor\" the GM gives you.",
            [
                new("know-a-guy", "Know a guy", "10-minute", "3/6/9 GRT", "Name the kind of person you need: you know them (3), they owe you (6), or owe you a real risk (9). Once each/adventure.") { Marquee = true },
                new("appraise", "Appraise", "standard", "2 GRT", "Handle a leaving: GM tells its street price, who'd want it most, and two facts (one true, one rumor)."),
                new("buyers-market", "Buyer's market", "passive", "—", "Leavings sell for 150%. Once per adventure, sell something too hot to move without it being traced to you."),
                new("papers", "Papers", "10-minute", "4 GRT", "REA/make to forge papers: 6+ street patrol, 7+ Cordon checkpoint, 9+ the Institute. Papers work once."),
                new("grease", "Grease", "free", "10 scrip/+1", "Bribing, persuading or deceiving officials: spend scrip like grit, +1 per 10 scrip (max +6). Money's gone."),
                new("leverage", "Leverage", "standard", "4 GRT", "REA/investigate to learn a secret about a named NPC. Hold it: CHA checks vs them with advantage."),
                new("flashback", "Flashback", "free", "1 hold/session", "Burn your hold to narrate a prep flashback: add details, advantage, a reroll, or the tool you need.") { SrdSource = "SRD (Agent)" },
            ],
            [
                new("Fence", 6, 8, 10, 6, 12, ["know-a-guy", "appraise", "buyers-market", "leverage"]),
                new("Smuggler", 8, 10, 6, 6, 12, ["know-a-guy", "papers", "grease", "flashback"]),
            ],
            ["good-suit", "contact-book", "forged-pass"],
            [
                new("Pocket gun", [new("Pocket revolver", ["pocket-revolver"]), new("Derringer", ["derringer"])]),
            ]),

        new("rigger", "Rigger", "Making leavings work, repairing anything",
            "Institute dropout, garage tinkerer, salvage rat. Leavings don't come with instructions.",
            "Curiosity", "When you see a leaving you've never seen before, you must pass a 6+ WIL/resist other check to leave it alone.",
            [
                new("make-it-sing", "Make it sing", "standard", "X GRT", "X = strangeness. REA/use tech vs 5 + X: learn trigger, one effect and cost. It's tuned for the whole crew.") { Marquee = true },
                new("jury-rig", "Jury-rig", "10-minute", "4 GRT", "Repair any broken device, recharge a cell (+d6 charges), or make something broken work for one scene."),
                new("safe-handling", "Safe handling", "free", "3 GRT", "Packed leavings never go off by accident. Burn 3 GRT to halve leaving/snag damage for all in close range."),
                new("instruments", "Instruments", "item", "—", "Detector rig: once per scene ask a yes/no question about fields or Spill activity in near range. Fragile."),
                new("field-theory", "Field theory", "passive", "—", "+2 REA/know about the Spill. Once per adventure, the GM tells you one way past a snag you've seen."),
                new("wrench-and-solder", "Wrench and solder", "standard", "X GRT", "Heal machines: an informatic being regains X + level HEA and is cured of non-Weird conditions."),
                new("hack", "Hack", "standard", "d10 GRT", "Tap any tech system. Looking is free; changes need REA/use tech: 6+ minor, 9+ major, 12+ takeover.") { SrdSource = "SRD (Mourner)" },
            ],
            [
                new("Bench rat", 6, 8, 12, 10, 6, ["make-it-sing", "field-theory", "instruments", "hack"]),
                new("Scrapper", 8, 10, 12, 4, 8, ["make-it-sing", "jury-rig", "safe-handling", "wrench-and-solder"]),
            ],
            ["toolkit", "tongs-gloves", "spare-cell-standard"],
            []),

        new("sawbones", "Sawbones", "Keeping people alive, chems, autopsies",
            "Disgraced surgeon, corner chemist, combat medic who never left the war.",
            "The oath", "If you leave a dying person untreated when you could have helped, you lose d6 GRT and can't regain pool from your next rest.",
            [
                new("back-from-the-brink", "Back from the brink", "standard", "10 GRT", "Within a minute of death, REA/heal 7+: alive, prone, 1 HEA; max HEA −2 more. Once per patient/adventure.") { Marquee = true },
                new("patch-job", "Patch job (heal)", "standard", "X GRT", "Target regains X + level HEA and is cured of non-Weird conditions. Wakes someone at 0 HEA. Twice/day each.") { SrdSource = "SRD (Mourner)" },
                new("chems", "Chems", "item", "1 dose", "d6 + level doses per adventure. A dose: regain d6 GRT, ignore fatigue 1 hr, or advantage on WIL/resist."),
                new("spill-medicine", "Spill medicine", "passive", "—", "Heal Spill-touch conditions, +2 to diagnose them, −2 exposure per treatment. Once/adventure undo a mark step."),
                new("triage", "Triage", "free", "3 GRT", "Once per round, give aid to someone within close range as a free action."),
                new("bedside-manner", "Bedside manner", "standard", "3 GRT", "Calm someone frightened or in shock: they regain d4 + level GRT and shake off fear. Once/person/day."),
                new("autopsy", "Autopsy", "10-minute", "2 GRT", "Learn how a body died, roughly when, and one thing they did in their last hour (and which snag, if Spill)."),
            ],
            [
                new("Disgraced surgeon", 4, 8, 12, 10, 8, ["back-from-the-brink", "patch-job", "spill-medicine", "autopsy"]),
                new("Corner chemist", 6, 10, 12, 6, 8, ["patch-job", "chems", "triage", "bedside-manner"]),
            ],
            ["medkit", "knife", "gloves-mask"],
            []),

        new("sensitive", "Sensitive", "Hearing the Spill, using its physics",
            "The Spill hums, and you can hear it. When you reach for it, it answers, but it takes something every time.",
            "The Spill mark", "Each time you use a Spill-touch (Weird) power, roll a d6. On a 1, your Spill mark advances one step.",
            [
                new("listen", "Listen", "standard", "3 GRT", "WIL/perceive: sense every active snag and leaving within far range, direction, strength and impression.") { Marquee = true },
                new("spill-touched", "Spill-touched", "varies", "varies", "Gain one Weird power, reskinned as Spill physics. At level-up you may take a Weird power instead.")
                    { Required = true, Choice = ChoiceKind.WeirdPower, ChoicePrompt = "Weird power" },
                new("lure", "Lure", "standard", "5 GRT", "CHA/control vs a Spill-creature's WIL (or 7+ for a snag): it drifts toward you or a point within near."),
                new("static", "Static", "free", "3 GRT", "Electronics and charged weapons in close range die until end of next round. Automatons: WIL 6+ or stunned."),
                new("echo", "Echo", "10-minute", "4 GRT", "Touch an object or place and roll WIL: relive the strongest emotional moment it has witnessed."),
                new("wrong-footed", "Wrong-footed", "passive", "—", "Once per scene, when a snag would affect you, roll WIL: on 7+ it passes over you."),
                new("detect", "Detect", "standard", "3 GRT", "Call bullshit: the GM must tell you whether something someone said is a lie.") { SrdSource = "SRD (Wytch)" },
            ],
            [
                new("Dowser", 4, 8, 10, 12, 8, ["listen", "spill-touched", "wrong-footed", "lure"], "Spill-touched: slip"),
                new("Medium", 8, 8, 10, 12, 4, ["listen", "spill-touched", "echo", "detect"], "Spill-touched: augur"),
            ],
            ["gas-mask", "lead-lined-case"],
            [])
        {
            Restrictions =
            [
                "You must take Spill-touched at 1st level.",
                "One of your scores must be a d4.",
                "You count as Weird for any power or scanner that detects the Weird.",
            ],
            RequiresD4 = true,
            CanLearnWeird = true,
        },
    ];

    // ---------------------------------------------------------------- Peoples

    static readonly WeaponDef HiddenArmGun = new("2d6", "Close–near", "AP 1, concealed arm-gun", "—", "");

    public static readonly PeopleDef[] Peoples =
    [
        new("human", "Human", "Native",
            "Halcyon was a human city before the Layover. Easy mode, except in the Spill.",
            [
                new("common", "Common", "passive", "—", "Scanners and checkpoints let you through. MOV/sneak with advantage in city crowds."),
            ],
            [
                new("born-to-die", "Born to die", "passive", "—", "Each time you lose 6+ HEA, gain 2 GRT (can exceed max).") { SrdSource = "SRD human" },
                new("free-of-fate", "Free of fate", "permanent", "—", "+1 to a score of your choice.") { Choice = ChoiceKind.Score, ChoicePrompt = "Score" },
                new("political", "Political", "passive", "—", "Advantage on one specific type of check tied to your background.") { Choice = ChoiceKind.Text, ChoicePrompt = "Which checks?" },
                new("protagonistic", "Protagonistic", "permanent", "—", "+6 max HEA or +4 max GRT.") { Choice = ChoiceKind.HeaOrGrt, ChoicePrompt = "Bonus" },
                new("runners-family", "Runner's family", "passive", "—", "Once per adventure, an old-timer passes you a rumor about the Spill."),
            ])
        { BestAt = "Blending in, flexibility", WorstAt = "Nothing special in the Spill" },

        new("spillborn", "Spillborn", "Mutated human",
            "The children of runners are born different. Most grew up in the shantytown on the Cordon's edge.",
            [
                new("home-ground", "Home ground", "passive", "—", "+2 to resist snags, ignore first exposure each run, tell a leaving from junk by touch."),
                new("marked", "Marked", "drawback", "—", "Weird to scanners; CHA with disadvantage vs city strangers who see it (not intimidate).") { Drawback = true },
            ],
            [
                new("dark-eyes", "Dark eyes", "permanent", "—", "You see in near-darkness as well as in daylight."),
                new("pelt-or-scale", "Pelt or scale", "permanent", "—", "DR 1 against non-Weird attacks.") { NaturalDr = 1 },
                new("quiet-feet", "Quiet feet", "passive", "—", "+2 on MOV/sneak checks."),
                new("unsettling", "Unsettling", "passive", "—", "CHA/intimidate checks with advantage."),
                new("not-all-there", "Not all there", "free", "2 GRT", "Once per scene when attacked: the attacker rolls twice and takes the lower."),
                new("snag-sense", "Snag-sense", "free", "1 GRT", "Know whether a snag lies within close range (not what kind)."),
            ])
        { BestAt = "Surviving the Spill", WorstAt = "Being trusted in the city" },

        new("cultivar", "Cultivar", "Grown construct",
            "Grown in a tank, built for a job. Your maker may still hold the lease on your body.",
            [
                new("made-for-purpose", "Made for purpose", "permanent", "—", "+1 to a score matching the job you were made for. Name your maker.") { Choice = ChoiceKind.Score, ChoicePrompt = "Score" },
                new("shelf-life", "Shelf life", "drawback", "20 scrip/adv.", "One maintenance dose per adventure; until taken, rest recovers only half pool.") { Drawback = true },
            ],
            [
                new("pain-dampers", "Pain dampers", "permanent", "—", "+6 max HEA.") { HeaBonus = 6 },
                new("fast-knit", "Fast knit", "passive", "—", "When you rest, recover an extra d6 HEA."),
                new("bespoke-face", "Bespoke face", "passive", "—", "Your first CHA check with anyone who has just met you has advantage."),
                new("pheromones", "Pheromones", "standard", "3 GRT", "Beasts in close range stop attacking d4 rounds; sapients give +2 on your next CHA."),
                new("imprinted-skill", "Imprinted skill", "passive", "—", "Advantage on one specific type of check, written into you at the tank.") { Choice = ChoiceKind.Text, ChoicePrompt = "Which checks?" },
                new("labor-graft", "Labor graft", "permanent", "—", "+2 on COM/grapple checks, and you can carry twice as much."),
            ])
        { BestAt = "Being built for the job", WorstAt = "Upkeep costs, owned by someone" },

        new("automaton", "Automaton", "Machine construct",
            "Machine people. You don't breathe, eat, or sleep, but every hospital will turn you away.",
            [
                new("machine-body", "Machine body", "permanent", "—", "Immune to poison, gas, disease. 3 hrs standby per 24; rest only in standby."),
                new("hard-to-mend", "Hard to mend", "drawback", "—", "No medicine; heal by rest, wrench and solder, or shops. EM resists at disadvantage.") { Drawback = true },
            ],
            [
                new("plated", "Plated", "permanent", "—", "DR 1 against non-Weird attacks.") { NaturalDr = 1 },
                new("perfect-recall", "Perfect recall", "passive", "—", "Record anything you see or hear and play it back perfectly."),
                new("built-in-tool", "Built-in tool", "permanent", "—", "One tool is part of you and can't be lost or taken.") { Choice = ChoiceKind.Text, ChoicePrompt = "Which tool?" },
                new("hidden-weapon", "Hidden weapon", "item", "—", "Concealed arm-gun (2d6, close–near, AP 1) that a pat-down won't find.") { GrantsWeapon = HiddenArmGun },
                new("logic-core", "Logic core", "permanent", "—", "+1 REA.") { ScoreBonus = Score.Rea },
                new("overclock", "Overclock", "free", "4 GRT", "MOV up one die type until end of round, then take 2 heat damage."),
            ])
        { BestAt = "Toxins, air, endurance", WorstAt = "Getting repaired, EM snags" },

        new("lumm", "Lumm", "Alien (heavy-world)",
            "Big, slow, dense aliens from a heavier world. They live three hundred years and remember everything.",
            [
                new("heavy-world-build", "Heavy-world build", "permanent", "—", "+6 max HEA and +2 on COM/grapple checks.") { HeaBonus = 6 },
                new("massive", "Massive", "drawback", "—", "No tight spaces or swimming; double damage from gravity snags; floors may give.") { Drawback = true },
            ],
            [
                new("bark-hide", "Bark hide", "permanent", "—", "DR 1 against non-Weird attacks.") { NaturalDr = 1 },
                new("long-memory", "Long memory", "passive", "—", "Once per session, ask the GM a question about Halcyon's past: a true answer."),
                new("infrasound", "Infrasound", "passive", "—", "CHA/intimidate with advantage. Talk to Lumm through walls, a block away."),
                new("patient", "Patient", "passive", "—", "Once per scene, spend a round only watching: your next check has advantage."),
                new("slow-blood", "Slow blood", "passive", "—", "Hold breath 10 min, a week without food; +2 WIL/resist world vs poison."),
                new("immovable", "Immovable", "passive", "—", "Nothing your size or smaller can knock you prone, push or grapple you."),
            ])
        { BestAt = "Toughness, memory", WorstAt = "Tight spaces, gravity snags" },

        new("quill", "Quill", "Alien (field-sensing)",
            "Light, narrow aliens covered in fine sensory filaments. The Spill is very loud to them.",
            [
                new("field-sense", "Field sense", "passive", "—", "GM warns you of charging energy weapons and EM/gravity snags in near range."),
                new("hollow-boned", "Hollow-boned", "drawback", "—", "Starting HEA is maxCOM + maxMOV − 4; +2 damage from falls and crushing.") { Drawback = true, HeaBonus = -4 },
            ],
            [
                new("glide", "Glide", "passive", "—", "No falling damage if you can spread your filaments; glide down from heights."),
                new("quick", "Quick", "permanent", "—", "+1 MOV.") { ScoreBonus = Score.Mov },
                new("mimic", "Mimic", "passive", "—", "Reproduce any sound or voice; radio or unseen deceptions have advantage."),
                new("wide-eyes", "Wide eyes", "passive", "—", "+2 on WIL/perceive checks."),
                new("molt", "Molt", "free", "—", "Once per scene, shed filaments to slip a grapple or restraint automatically."),
                new("storm-sense", "Storm sense", "passive", "—", "Know hours ahead when the Spill will change, and if a blowout warning is real."),
            ])
        { BestAt = "Sensing energy, agility", WorstAt = "Fragility" },
    ];

    // ---------------------------------------------------------------- Gear

    static GearDef W(string id, string name, string dmg, string range, string tags, string legality, int? price, string shots, string cat) =>
        new(id, name, GearKind.Weapon, price, tags) { Weapon = new(dmg, range, tags, legality, shots), Legality = legality, Category = cat };

    static GearDef A(string id, string name, int dr, string notes, string legality, int price) =>
        new(id, name, GearKind.Armor, price, notes) { ArmorDr = dr, Legality = legality, Category = "Armor" };

    static GearDef K(string id, string name, int? price, string notes = "", string cat = "Runner's kit") =>
        new(id, name, GearKind.Kit, price, notes) { Category = cat };

    public static readonly GearDef[] Gear =
    [
        // Iron
        W("pocket-revolver", "Pocket revolver", "2d4", "Close–near", "AP 1, cylinder 5, fast, small, concealable, loud", "Permit", 50, "cyl 5", "Iron"),
        W("service-revolver", "Service revolver", "2d6", "Close–near", "AP 2, cylinder 6, loud", "Permit", 80, "cyl 6", "Iron"),
        W("heavy-revolver", "Heavy revolver", "2d10", "Close–near", "AP 3, cylinder 5, kick, loud", "Permit", 120, "cyl 5", "Iron"),
        W("derringer", "Derringer", "2d3", "Close", "AP 1, magazine 2, fast, small, concealable, loud", "Legal", 40, "mag 2", "Iron"),
        W("autopistol", "Autopistol", "2d6", "Close–near", "AP 2, magazine 12, fast, small, jams, loud", "Permit", 90, "mag 12", "Iron"),
        W("lever-rifle", "Lever rifle", "2d8", "Near–beyond", "AP 3, magazine 8, two-handed, loud", "Permit", 150, "mag 8", "Iron"),
        W("bolt-rifle", "Bolt rifle, scoped", "2d12", "Near–beyond", "AP 4, magazine 5, awkward, slow, two-handed, loud", "Restricted", 220, "mag 5", "Iron"),
        W("double-barrel", "Double-barrel shotgun", "2d6/3d4", "Close–near", "Mag 2, cone (shot), heavy, two-handed, loud", "Legal", 90, "mag 2", "Iron"),
        W("sawn-off", "Sawn-off shotgun", "2d6", "Close", "Magazine 2, cone, concealable, loud", "Contraband", 70, "mag 2", "Iron"),
        W("pump-shotgun", "Pump shotgun", "2d6/3d4", "Close–near", "Mag 6, cone (shot), heavy, two-handed, loud", "Permit", 160, "mag 6", "Iron"),
        W("machine-pistol", "Machine pistol", "2d6", "Close–near", "Magazine 30, burst, jams, loud", "Contraband", 300, "mag 30", "Iron"),
        W("flare-pistol", "Flare pistol", "d6+burn 1", "Close–far", "Magazine 1, lights near range 1 min", "Legal", 15, "mag 1", "Iron"),
        // Light
        W("stinger", "Stinger (pocket stunner)", "2d6", "Close–near", "Charge 6, small, concealable, stun setting; misfire, Spill-shy, quiet", "Permit", 150, "charge 6", "Light"),
        W("arc-pistol", "Arc pistol", "3d6", "Close–near", "Charge 8, EMP, fast; misfire, Spill-shy, quiet", "Restricted", 450, "charge 8", "Light"),
        W("lancer", "Lancer (beam pistol)", "3d6", "Close–far", "Charge 10, AP 3, fast; misfire, Spill-shy, quiet", "Restricted", 600, "charge 10", "Light"),
        W("lancer-rifle", "Lancer rifle", "3d8", "Near–beyond", "Charge 6, AP 4, two-handed; misfire, Spill-shy", "Restricted", 900, "charge 6", "Light"),
        W("burner", "Burner (cutting torch)", "2d6+burn 1", "Close", "Charge 10, two-handed, cuts doors; misfire, Spill-shy", "Legal", 180, "charge 10", "Light"),
        W("pulse-carbine", "Pulse carbine", "2d10", "Close–far", "Charge 12, burst, heavy, two-handed; misfire, Spill-shy", "Contraband", 1200, "charge 12", "Light"),
        W("scatter-cannon", "Scatter cannon", "3d6", "Close–near", "Charge 4, cone, heavy, slow, two-handed, loud; misfire", "Contraband", 700, "charge 4", "Light"),
        W("disruptor", "Disruptor", "—", "Close–near", "Charge 4, kills electronics d4 rds, informatic stun 3", "Restricted", 300, "charge 4", "Light"),
        W("shock-baton", "Shock baton", "d4+stun 1", "Close", "Charge 10 (on hit), EMP", "Permit", 60, "charge 10", "Light"),
        // Close quarters
        W("knife", "Knife / switchblade", "d3", "Close", "Fast, small, throwable, concealable", "Legal", 3, "", "Close quarters"),
        W("crowbar", "Crowbar / pipe / wrench", "d4", "Close", "Also a tool", "Legal", 1, "", "Close quarters"),
        W("cosh", "Billy club / cosh", "d4", "Close", "Stun 1, concealable", "Legal", 4, "", "Close quarters"),
        W("entrenching-tool", "Entrenching tool", "d6", "Close", "Also a tool (dig, probe)", "Legal", 5, "", "Close quarters"),
        W("machete", "Machete", "d8", "Close", "Clears Spill growths", "Legal", 12, "", "Close quarters"),
        W("fire-axe", "Fire axe", "d8", "Close", "+1 dmg two-handed; breaks doors", "Legal", 15, "", "Close quarters"),
        W("knuckledusters", "Knuckledusters", "d4", "Close", "Small, concealable; trained unarmed for fight", "Legal", 4, "", "Close quarters"),
        // Thrown & explosive
        W("frag-grenade", "Frag grenade", "4d4", "Near", "Radius close; on a throw of 1 it lands near you", "Contraband", 40, "", "Thrown"),
        W("flashbang", "Flashbang", "0", "Near", "Stun 2 in close range unless WIL/resist world 6+", "Restricted", 30, "", "Thrown"),
        K("smoke-canister", "Smoke canister", 10, "Fills near range for 3 rounds", "Thrown"),
        K("mining-charge", "Mining charge", 50, "3d10 structures, 2d10 close; restricted", "Thrown"),
        K("bag-of-bolts", "Bag of bolts (20, ribboned)", 1, "Probe the path ahead"),
        // Armor
        A("heavy-long-coat", "Heavy long coat", 1, "Hides a holster", "Legal", 20),
        A("stab-vest", "Stab vest", 1, "Concealable; DR 2 vs blades and claws", "Legal", 25),
        A("ballistic-vest", "Ballistic vest", 2, "Concealable under a coat", "Permit", 80),
        A("riot-plate", "Riot plate", 3, "−1 non-dodge MOV; you look like a cop", "Restricted", 150),
        A("field-mesh-coat", "Field-mesh coat", 1, "DR 3 vs energy weapons", "Restricted", 350),
        A("hazard-suit", "Hazard suit", 1, "+2 resist gases/toxins; −1 MOV; contraband", "Contraband", 120),
        A("insulated-rig", "Insulated rig", 1, "Halves electric/EM damage", "Legal", 90),
        A("leaded-apron", "Leaded apron suit", 2, "Adv. vs radiation; −2 non-dodge MOV; heavy", "Legal", 200),
        A("exoframe", "Exoframe", 4, "Charge 3, Spill-shy; dead: DR 2, −2 MOV", "Contraband", 400),
        // Runner's kit
        K("gas-mask", "Gas mask + 3 filters", 15, "A filter lasts one run"),
        K("clicker", "Clicker (needle detector)", 60, "Ticks near radiation/fields; Spill-shy"),
        K("lead-lined-case", "Lead-lined case", 50, "One hand-sized leaving; inert, unscannable"),
        K("tongs-gloves", "Insulated tongs and gloves", 8, "Handle leavings without touching"),
        K("hand-crank-torch", "Hand-crank torch", 5, "Never fails in the Spill"),
        K("battery-torch", "Battery torch", 2, "Spill-shy"),
        K("chemlights", "Chemlights (6)", 2),
        K("flares", "Flares (3)", 3),
        K("rope", "Rope (30 m), climbing gear", 5),
        K("grapnel", "Grapnel launcher", 45, "Line out to far range"),
        K("map-common", "Old annotated map (common)", 20, "Trustworthy maps cut read the ground by 2"),
        K("map-famous", "Famous runner's map", 200),
        K("radio", "Short-range radio (pair)", 40, "Spill-shy"),
        K("rations", "Field rations (3 days)", 3),
        K("water-filter", "Water filter", 10),
        K("medkit", "Medkit (10 uses)", 15, "Heal without −2; refills 5"),
        K("toolkit", "Toolkit", 20, "Jury-rig / wrench and solder without −2"),
        K("body-bag", "Body bag", 2),
        K("spare-cell-small", "Spare cell (small)", 10, "", "Cells"),
        K("spare-cell-standard", "Spare cell (standard)", 20, "", "Cells"),
        K("spare-cell-heavy", "Spare cell (heavy)", 40, "", "Cells"),
        K("shelf-life-dose", "Cultivar maintenance dose", 20, "Shelf life", "Services"),
        // Vehicles
        new("bicycle", "Bicycle", GearKind.Vehicle, 40, "Quiet; fits under the fence") { Category = "Vehicles" },
        new("motorcycle", "Motorcycle", GearKind.Vehicle, 400, "Fast, loud") { Category = "Vehicles" },
        new("flatbed", "Old flatbed truck", GearKind.Vehicle, 900, "To the fence, never through") { Category = "Vehicles" },
        new("runabout", "Electric runabout", GearKind.Vehicle, 1100, "Dies in the Spill") { Category = "Vehicles" },
        // Class kit specials (not for sale)
        K("good-suit", "A good suit", null, "", "Class kit"),
        K("contact-book", "Contact book", null, "", "Class kit"),
        K("forged-pass", "Forged Cordon pass (single use)", null, "", "Class kit"),
        K("gloves-mask", "Gloves and mask", null, "", "Class kit"),
    ];

    // ---------------------------------------------------------------- Tables

    public static readonly string[] Oddments =
    [
        "A bolt with a red ribbon, from the first run you survived",
        "A pre-Layover tram ticket, never punched",
        "A deck of cards with one card missing (the queen of cups)",
        "A glass eye, not yours",
        "A photograph of a house inside the Spill, with a family on the porch",
        "A Lumm \"patience stone,\" worn smooth",
        "A shed Quill filament that still twitches near power lines",
        "A Cordon dog tag, name filed off",
        "A tin of cough drops that taste faintly of lilies",
        "A pocket watch that runs exactly one hour fast, always",
        "A cultivar barcode, peeled off someone's skin",
        "A key to a locker at Crown Central station",
        "A matchbook from the Regent Hotel, with a phone number",
        "A tooth, in a small velvet box",
        "A child's drawing of you, by a child you never met",
        "A brass automaton finger",
        "A House of the Return sermon card, annotated in anger",
        "A seashell that, held to the ear, sounds like a tram, not the sea",
        "Half a pawn ticket from the Glass Counter",
        "A small stone that's always warm (strangeness 1; worth 5 scrip)",
    ];

    public static readonly string[] SpillMarkTable =
    [
        "Your shadow lags half a second behind you.",
        "Clocks near you lose a minute an hour.",
        "Your hair grows in as fine silver wire.",
        "Dogs stop and stare at you. All of them.",
        "Everything tastes faintly of copper.",
        "Your reflection blinks a moment after you do.",
        "Rain doesn't quite touch you, and people notice.",
        "You hear the hum in your sleep, and sleepwalk toward the Spill.",
        "Frost forms on whatever your fingertips rest on.",
        "Photographs of you come out double-exposed.",
        "Small objects in your pockets swap places with other people's.",
        "You smell of ozone and lilies. Spill-creatures find you very interesting.",
    ];

    public static readonly string[] HumanGiven =
        ["Anka", "Bruno", "Cass", "Dmitri", "Edda", "Felix", "Greta", "Hal", "Ines", "Jonah", "Katya", "Lev", "Mira", "Nico", "Odile", "Piet", "Rosa", "Stas", "Tamsin", "Viktor"];

    public static readonly string[] HumanFamily =
        ["Varga", "Halloran", "Ostrowski", "Pell", "Marchetti", "Kowal", "Vane", "Dunmore", "Szabo", "Reyes", "Brannock", "Calloway", "Lindqvist", "Hollis", "Petrescu", "Okafor", "Hartigan", "Moreau", "Vogel", "Quarry"];

    public static readonly string[] StreetNames =
        ["Bolt", "Lucky", "Ribbon", "Halfpenny", "Socks", "Cinders", "Tin", "Grandpa", "Sparrow", "Deacon", "Lefty", "Twice"];

    public static readonly string[] SpillbornNicknames = ["Glass", "Owl", "Stilts", "Copper", "Fox", "Hush", "Pockets"];

    public static readonly (string Line, string Name)[] CultivarNames =
    [
        ("Hale Dancer III", "June"), ("Verrin Stevedore", "Marrow"), ("Atelier \"Companion\"", "Saint"), ("Brightwork Nanny", "Wren"),
        ("Verrin Bodyguard", "Ash"), ("Hale Singer", "Delphine"), ("Dockline Heavy", "Barrow"), ("Institute Fieldform", "Tess"),
        ("Atelier Host", "Clement"), ("Brightwork Clerk", "Penny"), ("Hale Courier", "Rook"), ("Discontinued (no record)", "\"Nobody\""),
    ];

    public static readonly (string Designation, string Name)[] AutomatonNames =
    [
        ("HSK-11 \"Housekeeper\"", "Mrs. Ellery"), ("Tramworks Unit 4", "Conductor"), ("Lamplighter 9", "Wick"), ("Institute Probe B", "Bee"),
        ("Customs Scanner 22", "Officer"), ("Dockloader 31", "Thirty"), ("Bellhop, Regent Hotel", "Mr. Augustine"), ("Mining Rig 6", "Dig"),
        ("Organ-tuner", "Fugue"), ("Cordon Sentry (decommissioned)", "Private"), ("No designation at all", "Lantern"), ("Scratched off with a file", "New Name Weekly"),
    ];

    public static readonly string[] LummNames = ["Oruun", "Haddoth", "Mmbe", "Turro", "Gaavul", "Dumm", "Orrava", "Bhuun", "Kolloth", "Uumae", "Rhoddu", "Vohm"];

    public static readonly string[] QuillNames = ["Tsiri", "Iiv", "Fessa", "Ki'ith", "Aesh", "Quiet-Then-Loud", "Shee-lo", "Tik-ast", "Vrrin", "Lisp", "Sennet", "Filament"];

    // ---------------------------------------------------------------- Lookups

    public static ClassDef? FindClass(string? id) => Classes.FirstOrDefault(c => c.Id == id);
    public static PeopleDef? FindPeople(string? id) => Peoples.FirstOrDefault(p => p.Id == id);
    public static GearDef? FindGear(string? id) => Gear.FirstOrDefault(g => g.Id == id);
    public static PowerDef? FindWeird(string? idOrName) =>
        WeirdPowers.FirstOrDefault(w => w.Id == idOrName || string.Equals(w.Name, idOrName, StringComparison.OrdinalIgnoreCase));

    /// <summary>Find any power definition by id across classes, peoples, advanced and Weird lists.</summary>
    public static PowerDef? FindPower(string id) =>
        Classes.SelectMany(c => c.Powers)
            .Concat(Peoples.SelectMany(p => p.GainAll.Concat(p.Choose)))
            .Concat(AdvancedPowers)
            .Concat(WeirdPowers)
            .FirstOrDefault(p => p.Id == id);

    public static string ScoreName(Score s) => s.ToString().ToUpperInvariant();

    public static readonly (Score Score, string Long, string Uses)[] ScoreInfo =
    [
        (Score.Com, "combat", "attack · grapple"),
        (Score.Mov, "movement", "dodge · sneak · climb · flee · drive · initiate · parry"),
        (Score.Rea, "reason", "heal · investigate · know · make/repair · use tech · Spill powers"),
        (Score.Wil, "will", "perceive · empathize · resist other · resist world"),
        (Score.Cha, "charm", "control · intimidate · negotiate · seduce"),
    ];
}
