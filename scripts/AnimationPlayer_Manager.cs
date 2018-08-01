using Godot;
using System;

public class AnimationPlayer_Manager : AnimationPlayer
{

    private Dictionary<string, string[]> states = new Dictionary<string, string[]>()
    {
        { "Idle_unarmed", new string[] { "Knife_equip", "Pistol_equip", "Rifle_equip", "Idle_unarmed" } },

        { "Pistol_equip", new string[] { "Pistol_idle" } },
        { "Pistol_fire", new string[] { "Pistol_idle" } },
        { "Pistol_idle", new string[] { "Pistol_fire", "Pistol_reload", "Pistol_unequip", "Pistol_idle" } },
        { "Pistol_reload", new string[] { "Pistol_idle" } },
        { "Pistol_unequip", new string[] { "Idle_unarmed" } },

        { "Rifle_equip", new string[] { "Rifle_idle" } },
        { "Rifle_fire", new string[] { "Rifle_idle" } },
        { "Rifle_idle", new string[] { "Rifle_fire", "Rifle_reload", "Rifle_unequip", "Rifle_idle" } },
        { "Rifle_reload", new string[] { "Rifle_idle" } },
        { "Rifle_unequip", new string[] { "Idle_unarmed" } },

        { "Knife_equip", new string[] { "Knife_idle" } },
        { "Knife_fire", new string[] { "Knife_idle" } },
        { "Knife_idle", new string[] { "Knife_fire", "Knife_unequip", "Knife_idle" } },
        { "Knife_unequip", new string[] { "Idle_unarmed" } }
    };

    private Dictionary<string, float> animation_speeds = new Dictionary<string, float>()
    {
        { "Idle_unarmed", 1f },

        { "Pistol_equip", 1.4f },
        { "Pistol_fire", 1.8f },
        { "Pistol_idle", 1f },
        { "Pistol_reload", 1f },
        { "Pistol_unequip", 1.4f },

        { "Rifle_equip", 2f },
        { "Rifle_fire", 6f },
        { "Rifle_idle", 1f },
        { "Rifle_reload", 1.45f },
        { "Rifle_unequip", 2f },

        { "Knife_equip", 1f },
        { "Knife_fire", 1.35f },
        { "Knife_idle", 1f },
        { "Knife_unequip", 1f }
    };

    private string current_state = null;
    private FuncRef callback_function = null;

    public override void _Ready()
    {

        SetAnimation("Idle_unarmed");
        Connect("animation_finished", this, "animation_ended");
        
    }

    private bool SetAnimation(string animation_name)
    {
        if(animation_name == current_state)
        {
            GD.Print("AnimationPlayer_Manager.gd -- WARNING: animation is already ", animation_name);
            return true;
        }

        if (HasAnimation(animation_name))
        {

            if(current_state != null)
            {

                var possible_animations = states[current_state];
                foreach(var animation in possible_animations)
                {

                    if(animation_name == animation)
                    {
                        current_state = animation_name;
                        Play(animation_name, -1, animation_speeds[animation_name]);
                        return true;
                    }
                    else
                    {
                        GD.Print("AnimationPlayer_Manager.gd -- WARNING: Cannot change to ", animation_name, " from ", current_state);
                        return false;
                    }

                }

            }
            else
            {

                current_state = animation_name;
                Play(animation_name, -1, animation_speeds[animation_name]);
                return true;

            }

        }

        return false;

    }

    private void AnimationEnded(string anim_name)
    {

        /*UNARMED transitions*/
        if (current_state == "Idle_unarmed")
            return;
        /*KNIFE transitions*/
        else if (current_state == "Knife_equip")
            SetAnimation("Knife_idle");
        else if (current_state == "Knife_idle")
            return;
        else if (current_state == "Knife_fire")
            SetAnimation("Knife_idle");
        else if (current_state == "Knife_unequip")
            SetAnimation("Idle_unarmed");
        /*PISTOL transitions*/
        else if (current_state == "Pistol_equip")
            SetAnimation("Pistol_idle");
        else if (current_state == "Pistol_idle")
            return;
        else if (current_state == "Pistol_fire")
            SetAnimation("Pistol_idle");
        else if (current_state == "Pistol_unequip")
            SetAnimation("Idle_unarmed");
        else if (current_state == "Pistol_reload")
            SetAnimation("Pistol_idle");
        /*RIFLE transitions*/
        else if (current_state == "Rifle_equip")
            SetAnimation("Rifle_idle");
        else if (current_state == "Rifle_idle")
            return;
        else if (current_state == "Rifle_fire")
            SetAnimation("Rifle_idle");
        else if (current_state == "Rifle_unequip")
            SetAnimation("Pistol_idle");
        else if (current_state == "Pistol_fire")
            SetAnimation("Idle_unarmed");
        else if (current_state == "Rifle_reload")
            SetAnimation("Rifle_idle");

    }

    private void AnimationCallback()
    {
        if (callback_function == null)
            GD.Print("AnimationPlayer_Manager.gd -- WARNING: No callback function for the animation to call!");
        else
            callback_function.CallFunc();
    }

}
