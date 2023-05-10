using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.Json;

namespace HypixelAPIProgram
{
    public partial class Form1 : Form
    {
        private const string MOJANG_API_URL = "https://api.mojang.com/users/profiles/minecraft/";
        private const string HYPIXEL_API_URL = "https://api.hypixel.net/player";
        

        public Form1()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void btnGetData_ClickAsync(object sender, EventArgs e)
        {
            string apiKey = txtApiKey.Text;
            string playerName = txtPlayerName.Text;
            string uuid = await GetUuid(playerName);
            if (comboBox1.SelectedItem != null)
            {
                if (uuid == null)
                {
                    MessageBox.Show("Invalid player name.");
                    return;
                }

                Dictionary<string, string> playerData = GetBedwarsData(apiKey, playerName, comboBox1.SelectedText);
                if (playerData == null)
                {
                    MessageBox.Show("Failed to get player data.");
                    return;
                }
                // 将玩家数据添加到 DataGridView 中
                dgvPlayerData.Rows.Clear();
                foreach (KeyValuePair<string, string> entry in playerData)
                {
                    dgvPlayerData.Rows.Add(entry.Key, entry.Value);
                }
            }
            else
            {
                MessageBox.Show("你应该选择模式！");
                return;
            }
            
        }
        private async Task<string> GetUuid(string playerName)
        {
            string url = MOJANG_API_URL + playerName;
            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    JsonElement json = JsonSerializer.Deserialize<JsonElement>(responseBody);
                    return json.GetProperty("id").GetString();
                }
                else
                {
                    return null;
                }
            }
        }
        private Dictionary<string, string> GetBedwarsData(string apiKey, string playerId, string mode)
        {
            // 构造 Mojang API 获取 UUID 的 URL
            var mojangUrl = $"https://api.mojang.com/users/profiles/minecraft/{playerId}";
            // 发送 GET 请求获取玩家 UUID
            var client = new HttpClient();
            var mojangResponse = client.GetAsync(mojangUrl).Result;
            var mojangContent = mojangResponse.Content.ReadAsStringAsync().Result;
            var mojangJson = JsonSerializer.Deserialize<JsonElement>(mojangContent);
            var MUUID = mojangJson.GetProperty("id").GetString();


            // 构造 Hypixel API 获取玩家数据的 URL
            var hypixelUrl = $"https://api.hypixel.net/player?key={apiKey}&uuid={MUUID}&fields=stats.Bedwars";



            // 发送 GET 请求获取玩家 Bedwars 数据
            var hypixelResponse = client.GetAsync(hypixelUrl).Result;
            var hypixelContent = hypixelResponse.Content.ReadAsStringAsync().Result;
            var hypixelJson = JsonSerializer.Deserialize<JsonElement>(hypixelContent);
            var bedwarsStats = hypixelJson.GetProperty("player").GetProperty("stats").GetProperty("Bedwars");
            if (mode == "all")
            {
                // 将玩家 Bedwars 数据添加到字典中
                var bedwarsallData = new Dictionary<string, string>();
                bedwarsallData.Add("玩家名字", playerId);
                bedwarsallData.Add("硬币", bedwarsStats.GetProperty("coins").GetString());
                bedwarsallData.Add("总游戏次数", bedwarsStats.GetProperty("games_played_bedwars_1").GetString());
                bedwarsallData.Add("胜利次数", bedwarsStats.GetProperty("wins_bedwars").GetString());
                bedwarsallData.Add("失败次数", bedwarsStats.GetProperty("losses_bedwars").GetString());
                bedwarsallData.Add("总杀敌数", bedwarsStats.GetProperty("kills_bedwars").GetString());
                bedwarsallData.Add("总死亡数", bedwarsStats.GetProperty("deaths_bedwars").GetString());
                bedwarsallData.Add("床摧毁次数", bedwarsStats.GetProperty("beds_broken_bedwars").GetString());
                bedwarsallData.Add("床丢失次数", bedwarsStats.GetProperty("beds_lost_bedwars").GetString());
                bedwarsallData.Add("最终击杀数", bedwarsStats.GetProperty("final_kills_bedwars").GetString());
                bedwarsallData.Add("最终死亡数", bedwarsStats.GetProperty("final_deaths_bedwars").GetString());
                return bedwarsallData;
            }
            else if (mode == "4v4")
            {
                // 将玩家 bw4v4 数据添加到字典中
                var bedwars4v4Data = new Dictionary<string, string>();
                bedwars4v4Data.Add("玩家名字", playerId);
                bedwars4v4Data.Add("总游戏次数", bedwarsStats.GetProperty("four_four_games_played_bedwars").GetString());
                bedwars4v4Data.Add("胜利次数", bedwarsStats.GetProperty("four_four_wins_bedwars").GetString());
                bedwars4v4Data.Add("失败次数", bedwarsStats.GetProperty("four_four_losses_bedwars").GetString());
                bedwars4v4Data.Add("总杀敌数", bedwarsStats.GetProperty("four_four_kills_bedwars").GetString());
                bedwars4v4Data.Add("总死亡数", bedwarsStats.GetProperty("four_four_deaths_bedwars").GetString());
                bedwars4v4Data.Add("床摧毁次数", bedwarsStats.GetProperty("four_four_beds_broken_bedwars").GetString());
                bedwars4v4Data.Add("床丢失次数", bedwarsStats.GetProperty("four_four_beds_lost_bedwars").GetString());
                bedwars4v4Data.Add("最终击杀数", bedwarsStats.GetProperty("four_four_final_kills_bedwars").GetString());
                bedwars4v4Data.Add("最终死亡数", bedwarsStats.GetProperty("four_four_final_deaths_bedwars").GetString());
                return bedwars4v4Data;
            }
            else if (mode == "solo")
            {
                // 将玩家 bw4v4 数据添加到字典中
                var bedwarssoloData = new Dictionary<string, string>();
                bedwarssoloData.Add("玩家名字", playerId);
                bedwarssoloData.Add("等级", bedwarsStats.GetProperty("level").GetString());
                bedwarssoloData.Add("总游戏次数", bedwarsStats.GetProperty("games_played_bedwars_1").GetString());
                bedwarssoloData.Add("胜利次数", bedwarsStats.GetProperty("wins_bedwars").GetString());
                bedwarssoloData.Add("失败次数", bedwarsStats.GetProperty("losses_bedwars").GetString());
                bedwarssoloData.Add("胜率", bedwarsStats.GetProperty("win_ratio_bedwars").GetString());
                bedwarssoloData.Add("总杀敌数", bedwarsStats.GetProperty("kills_bedwars").GetString());
                bedwarssoloData.Add("总死亡数", bedwarsStats.GetProperty("deaths_bedwars").GetString());
                bedwarssoloData.Add("K/D 比", bedwarsStats.GetProperty("k_d").GetString());
                bedwarssoloData.Add("床摧毁次数", bedwarsStats.GetProperty("beds_broken_bedwars").GetString());
                bedwarssoloData.Add("床丢失次数", bedwarsStats.GetProperty("beds_lost_bedwars").GetString());
                bedwarssoloData.Add("最终击杀数", bedwarsStats.GetProperty("final_kills_bedwars").GetString());
                bedwarssoloData.Add("最终死亡数", bedwarsStats.GetProperty("final_deaths_bedwars").GetString());
                bedwarssoloData.Add("最终 K/D 比", bedwarsStats.GetProperty("final_k_d").GetString());
                return bedwarssoloData;
            }




        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
