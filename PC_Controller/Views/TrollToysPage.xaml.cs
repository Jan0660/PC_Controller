using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Communication.Main;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PC_Controller.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrollToysPage : ContentPage
    {
        string[] commands = new string[] { "START_PROCESS:cmd.exe|/c cd C:/Windows/System32 & tree", "START_PROCESS:winver|",
"START_PROCESS:taskmgr|", "OPEN_MSGBOX:calling a simple visual basic script that opens message boxes a virus|Let me introduce you to my religion",
"START_PROCESS:cmd.exe|/k vol", "OPEN_NOTEPAD_WITH_TEXT:You done fucked up",
"START_PROCESS:https://youtu.be/WH2GIiqIctI|", "START_PROCESS:https://youtu.be/YyzMb8Xb2oI|",
"OPEN_MSGBOX:wtf is star wars|hello there", "START_PROCESS:https://youtu.be/LSgk7ctw1HY?t=80|",
"START_PROCESS:https://yandere-simulator.com/tampon.png|", "START_PROCESS:https://youtu.be/pkubIfC6w5A|",
"START_PROCESS:https://youtu.be/tGqVMbAQhBs?t=246|", "START_PROCESS:https://cdn.discordapp.com/attachments/583014457848889346/740323770203635892/image0.jpg|",
"START_PROCESS:https://media.discordapp.net/attachments/438793072697016331/741766342209830942/5.png|",
"START_PROCESS:https://cdn.discordapp.com/attachments/686015484281225241/732185242353401936/unknown.png|",
"START_PROCESS:https://cdn.discordapp.com/attachments/686015484281225241/732185242353401936/unknown.png|",
"OPEN_NOTEPAD_WITH_TEXT:REEEEEEEEEEEEEEEEEEEeEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEeEEEEEEEEEEEEEEEEEEEEEEEEEEEEEeEEEEEEEEEEEEEEeee",
"OPEN_MSGBOX:So NVidia, FUCK YOU! - Linus Torvalds|He actually said this.", "OPEN_MSGBOX:Message|Title",
"OPEN_MSGBOX:r/SoftwareGore is a retarded subreddit because error message can be easily faked ./.|the truth",
@"OPEN_NOTEPAD_WITH_TEXT:What the fuck did you just fucking say about me, you little bitch?
I'll have you know I graduated top of my class in the Navy Seals, and I've been involved in numerous secret raids on Al-Quaeda, and I have over 300 confirmed kills.
I am trained in gorilla warfare and I'm the top sniper in the entire US armed forces.
You are nothing to me but just another target.
I will wipe you the fuck out with precision the likes of which has never been seen before on this Earth, mark my fucking words.
You think you can get away with saying that shit to me over the Internet? Think again, fucker.
As we speak I am contacting my secret network of spies across the USA and your IP is being traced right now so you better prepare for the storm, maggot.
The storm that wipes out the pathetic little thing you call your life. You're fucking dead, kid.
I can be anywhere, anytime, and I can kill you in over seven hundred ways, and that's just with my bare hands. Not only am I extensively trained in unarmed combat, but I have access to the entire arsenal of the United States Marine Corps and I will use it to its full extent to wipe your miserable ass off the face of the continent, you little shit.
If only you could have known what unholy retribution your little ""clever"" comment was about to bring down upon you, maybe you would have held your fucking tongue.
But you couldn't, you didn't, and now you're paying the price, you goddamn idiot. I will shit fury all over you and you will drown in it. You're fucking dead, kiddo.", "OPEN_MSGBOX:minecraft good, fortnite bad, keanu reeves, wholesome 100, 69 420 haha funni number|REDDIT MOMENT",
@"OPEN_NOTEPAD_WITH_TEXT:reddit reposting intensifies
Edit: thank you for the gold kind stranger!
Edit 2: fix typo
Edit 3: holy fucking shit how did this get so many upvotes
Edit 4: omfg my dms are fucking rekt'd i forgot to run off notifications omg
Edit 5: minecraft good, fortnite bad, keanu reeves, wholesome 100, 69, 420, elon musk
Edit 6: FOLLOW ME ON WHATEVER FUCKING SOCIAL SITE I PUT HERE - https://github.com/Jan0660
Edit 7: Covid20SX+", "OPEN_MSGBOX:Exception handled succesfully|Unhandled exception ocurred",
"OPEN_MSGBOX:hello|hi"};
        Random random = new Random();
        
        public TrollToysPage()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            //show/hide console window
            SendData("SHOW_HIDE_WINDOW");
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            SendData("START_PROCESS:https://youtu.be/dQw4w9WgXcQ|");
            ReceiveData();
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            for (int i = 0; i < Convert.ToInt32(slider.Value); i++)
            {
                int ree = random.Next(commands.Length);
                SendData(commands[ree]);
                if (commands[ree].StartsWith("START_PROCESS:"))
                {
                    ReceiveData();
                }
                await Task.Delay(150);
            }
        }

        private void slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            if (Convert.ToInt32(e.NewValue) != 1)
            {
                button.Text = "Open " + Convert.ToInt32(e.NewValue).ToString() + " random things";
            }
            else
            {
                button.Text = "Open " + Convert.ToInt32(e.NewValue).ToString() + " random thingsssssss";
            }
        }

        private void Button_Clicked_3(object sender, EventArgs e)
        {
            SendData("OPEN_MSGBOX:" + MsgBoxMessageEntry.Text + "|" + MsgBoxTitleEntry.Text);
        }

        private void Button_Clicked_4(object sender, EventArgs e)
        {
            string screenBounds = Request("GET_SCREEN_BOUNDS");
            int width = int.Parse(screenBounds.Split('|')[0]);
            int height = int.Parse(screenBounds.Split('|')[1]);
            SendData("SET_MOUSE_LOCATION:" + random.Next(width) + "|" + random.Next(height));
        }

        private void Button_Clicked_5(object sender, EventArgs e)
        {
            SendData("OPEN_NOTEPAD_WITH_TEXT:" + NotepadTextEditor.Text);
        }
    }
}