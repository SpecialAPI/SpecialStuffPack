using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AkSoundShitgine = AkSoundEngine;

namespace SpecialStuffPack.Items.Guns
{
    public class AkSoundEngineGun : GunBehaviour
    {
        public const string ChatGPTWwiseArticle = 
            "As an audio professional in the gaming industry, you may have encountered Wwise, a popular audio middleware tool that allows you to integrate sound effects, music, and dialogue into your game. However, despite its widespread use, there are several reasons why you should consider moving away from Wwise. In this article, we’ll explore 10 reasons to stop using Wwise and consider alternative solutions.\n\n" +
            "1. Cost\n" +
            "One of the biggest drawbacks of Wwise is its cost.Wwise is a subscription-based service, with pricing tiers based on the number of audio assets and platforms you’re targeting. If you’re working on a large-scale project, the costs can quickly add up. This can be a significant barrier for indie developers or small studios on a tight budget.\n\n" +
            "2. Learning Curve\n" +
            "Wwise has a steep learning curve, and it can take time to master its features and workflows.The software is complex, and the documentation can be overwhelming for new users.This can be a significant frustration, particularly if you’re working under tight deadlines.\n\n" +
            "3. Limited Platform Support\n" +
            "Wwise’s platform support is limited to specific platforms, which can be a significant disadvantage for developers working on cross-platform projects.If you’re targeting platforms that are not supported by Wwise, you may need to use multiple middleware solutions, which can increase complexity and development time.\n\n" +
            "4. Performance Overhead\n" +
            "Wwise can have a significant performance overhead, particularly on lower-end hardware. The software uses a lot of CPU and memory resources, which can impact the overall performance of your game.This can be a significant issue for developers working on mobile or console platforms, where performance is critical.\n\n" +
            "5. Integration Issues\n" +
            "Wwise can have integration issues with other game engines, particularly if you’re using a custom engine.The software is designed to work with specific engines, and if you’re using a different engine, you may need to create custom integration code.This can be a significant issue, particularly if you’re working on a large-scale project with a complex codebase.\n\n" +
            "6. Limited Audio Editing Features\n" +
            "While Wwise is an excellent tool for integrating audio into your game, it has limited audio editing features. If you need to perform complex audio editing tasks, you may need to use a separate audio editing tool, which can be time-consuming and increase development time.\n\n" +
            "7. Limited Sound Design Capabilities\n" +
            "Wwise has limited sound design capabilities, which can be a significant disadvantage for sound designers. If you need to create complex soundscapes or perform advanced sound design tasks, you may need to use additional sound design tools, which can increase development time.\n\n" +
            "8. Limited Asset Management Features\n" +
            "Wwise has limited asset management features, particularly for large-scale projects. If you’re working on a project with a large number of audio assets, it can be challenging to manage them effectively using Wwise. This can result in a disorganized and confusing project structure.\n\n" +
            "9. Limited Collaboration Features\n" +
            "Wwise has limited collaboration features, which can be a significant issue for teams working remotely or in different locations. If you need to collaborate on audio assets with other team members, you may need to use external collaboration tools, which can increase complexity and development time.\n\n" +
            "10. Alternative Solutions\n" +
            "There are many alternative solutions to Wwise, including other audio middleware tools and custom audio engines.These solutions offer unique features and workflows that may be more suitable for your specific project needs. By exploring alternative solutions, you may find a more cost-effective, efficient, and flexible audio middleware solution.\n\n" +
            "In conclusion, while Wwise has been a popular audio middleware tool for many years, it has several significant drawbacks that can make it a less than ideal solution for some developers. By considering alternative solutions, you may find a more cost-effective, efficient, and flexible audio middleware tool that better suits your project needs.";

        public const string StopDoingWwise =
            "STOP DOING WWISE\n" +
            " * SOUNDS WERE NOT SUPPOSED TO BE SWITCHED\n" +
            " * YEARS OF USING WWISE yet NO REAL-WORLD USE FOUND FOR DOING ANYTHING BEYOND UNITY'S BUILTIN SOUND SYSTEM\n" +
            " * Wanted to use audio switches anyways? We had a tool for that: It was called \"custom code\"\n" +
            " * \"Yes please give me rtpc of something. Please give me Init.bnk of it\" - Statements dreamed up by the utterly Deranged.\n" +
            "LOOK at what Wwise developers have been demanding your Respect for all this time, with all the sounds and events we built for them\n" +
            "(This is REAL Wwise, done by REAL Wwise users):\n" +
            "{Wwise Screenshot}\n" +
            "     ?????\n" +
            "{Wwise Screenshot}\n" +
            "     ???????\n" +
            "{Wwise Screenshot}\n" +
            "??????????????????????\n" +
            "\"Hello I would like {Wwise logo} apples please\"\n" +
            "They have played us for absolute fools";

        public const string ChatGPTWwiseRant =
            "Look, let me tell you about Wwise, and why it's a steaming pile of garbage. I'm tired of hearing people praise this abomination as if it's some kind of audio holy grail. Well, newsflash, it's not!\n\n" +
            "First off, let's talk about the interface. Whoever designed it must have been high on some serious drugs. It's cluttered, confusing, and just plain unintuitive. I mean, who thought it was a good idea to hide important features behind a maze of menus and submenus? It's like they wanted to make the simplest tasks as convoluted as possible. Bravo, Wwise, bravo.\n\n" +
            "And don't even get me started on the documentation. Or should I say, the lack thereof. It's a jumbled mess of half-baked tutorials and outdated information.Good luck trying to find any helpful resources when you're stuck on a problem. Oh, and let's not forget about the community forums, where questions go unanswered and frustration runs rampant. It's like a black hole of knowledge sucking away any hope of getting help.\n\n" +
            "But wait, there's more! How about the constant bugs and glitches? It's like Wwise has a personal vendetta against anyone trying to use it. You spend hours meticulously setting up your audio project, only to have it crash and burn because of some random error that makes no sense whatsoever. And good luck trying to get support from the developers. It's like pulling teeth. They'll blame you for the issues, claiming it's user error, when it's clearly their broken software.\n\n" +
            "And let's not forget about the licensing fees. Wwise thinks it's entitled to drain your wallet dry for the privilege of using their inferior software. It's a money-grabbing scheme that preys on unsuspecting developers who just want to create great audio experiences. Shame on you, Wwise, for valuing profit over quality.\n\n" +
            "In conclusion, Wwise is a hot mess of a tool that should be avoided at all costs. It's a headache-inducing nightmare that will leave you questioning your career choices. Save yourself the agony and find a better audio solution. Trust me, you'll thank me later.";

        public const string WwiseBrutalOrchestra = "Wwise? More like brutal orchestra Giggling Minister theme. There's a reason why Brutal Orchestra uses FMOD and not Wwise. I really hope that when Wwise is fighting a Giggling Minister the 87% chance for Mind Games to do nothing fails and kills Wwise.";

        public static void Init()
        {
            string name = "AK-SoundEngine";
            string shortdesc = "The worst of the worst";
            string longdesc = $"Fires garbage. Very rapidly.\n\n{ChatGPTWwiseRant}";
            var gun = EasyGunInit("soundengine", name, shortdesc, longdesc, "soundengine_idle_001", "soundengine_idle_001", "gunsprites/aksoundengine/", 500, 1f, new(27, 5), Empty,
                "audioshitnetic", PickupObject.ItemQuality.C, GunClass.POISON, out var finish);
            gun.SetAnimationFPS(gun.reloadAnimation, 16);
            gun.AddComponent<AkSoundEngineGun>().switches = PickupObjectDatabase.Instance.Objects.OfType<Gun>().Select(x => x.gunSwitchGroup).ToList();
            var trashbag = GetProjectile(154);
            var trash = trashbag.GetComponent<SpawnProjModifier>().projectileToSpawnOnCollision;
            gun.RawSourceVolley.projectiles.Add(new()
            {
                angleVariance = 10f,
                projectiles = new()
                {
                    trashbag,
                    trash,
                    trash
                },
                numberOfShotsInClip = 30,
                shootStyle = ProjectileModule.ShootStyle.Automatic,
                cooldownTime = 0.11f
            });
            finish();
        }

        public override void OnPostFired(PlayerController player, Gun gun)
        {
            base.OnPostFired(player, gun);
            gun.gunSwitchGroup = switches.RandomElement();
            AkSoundShitgine.SetSwitch("WPN_Guns", gun.gunSwitchGroup, gun.gameObject);
            if (PlayerOwner.PlayerHasActiveSynergy("#ADVANCED_FEATURE-RICH_INTERACTIVE"))
            {
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.DefaultPoisonGoop).TimedAddGoopCircle(gun.barrelOffset.position, 1f, 0.5f, false);
            }
        }

        public override void OnReloadPressed(PlayerController player, Gun gun, bool manual)
        {
            base.OnReloadPressed(player, gun, manual);
            if (gun.IsReloading)
            {
                DeadlyDeadlyGoopManager.GetGoopManagerForGoopType(GoopDatabase.DefaultPoisonGoop).TimedAddGoopCircle(player.specRigidbody.UnitBottomCenter, PlayerOwner.PlayerHasActiveSynergy("#ADVANCED_FEATURE-RICH_INTERACTIVE") ? 4.5f : 2f, 0.5f, false);
            }
        }

        public List<string> switches;
    }
}
