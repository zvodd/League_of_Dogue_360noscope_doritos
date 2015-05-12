using System;
using System.Collections.Generic;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

namespace League_of_Dogue_360noscope_doritos
{
    class Program
    {
        struct minsprite{
            public bool is_active;
            public int netid;
            public Vector2 screenPos;
        }

        static private int minion_buffer_size = 60;
        static private Render.Sprite[] sprites = new Render.Sprite[minion_buffer_size];
        static private minsprite[] mins = new minsprite[minion_buffer_size];
        static private int minion_draw_stop_index = 0;
        
        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
        }

        private static void OnLoad(EventArgs args)
        {
            Game.OnUpdate += Game_OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            for (int i = 0; i < minion_buffer_size; i++)
            {
                Render.Sprite sp = new Render.Sprite(Resource1.dogue, Vector2.Zero);
                sp.Scale = new Vector2(0.25f,0.25f);
                sp.Add();
                sp.Hide();
                sprites[i] = sp;
            }

        }

        static void Drawing_OnDraw(EventArgs args)
        {
            int i = 0;
            // draw all minions in buffer (until minion_draw_stop_index)
            for (; i <= minion_draw_stop_index; i++)
            {
                if (mins[i].is_active) // <- Likely redundant
                {
                    // position and show sprites.
                    Render.Sprite sprite = sprites[i];
                    sprite.X = (int)mins[i].screenPos.X - (sprite.Width / 2);
                    sprite.Y = (int)mins[i].screenPos.Y - (sprite.Width / 2);
                    sprite.Show();
                }
                else 
                {
                    sprites[i].Hide(); // <- Likely redundant
                }
            }
            // clear the rest
            for (;i < minion_buffer_size; i++){
                sprites[i].Hide();

            }
        }
        static void Game_OnUpdate(EventArgs args)
        {

            int m_i = 0;
            // find all minions
            foreach (Obj_AI_Minion min in ObjectManager.Get<Obj_AI_Minion>())
            {
                
                if (min.IsValid && !min.IsDead && min.IsMinion && min.IsVisible) 
                    // TODO: not ward?
                {
                    // if minion is on screen
                    Vector2 xy = Drawing.WorldToScreen(min.Position);
                    if ((xy.X <= Drawing.Width && xy.X >= 0
                        && xy.Y <= Drawing.Height && xy.Y >= 0))
                    {
                        if (m_i == minion_buffer_size) { break; } // 
                        mins[m_i].is_active = true;
                        mins[m_i].screenPos = xy;
                        mins[m_i].netid = min.NetworkId;
                        m_i++;
                    }
                }
                    
            }
            minion_draw_stop_index = m_i -1;
        }
        
    }
}