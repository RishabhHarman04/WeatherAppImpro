using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using System.Net.Http.Headers;


namespace WindowsFormsApp5
{
    public partial class WeatherInfo : Form
    {
        private int panelIndex = 1;
        private HttpClient httpClient = new HttpClient();
        private string apiKey = MyStrings.Apikey;
        private Timer timer = new Timer();
        private int selectedInterval = 10000;


        public WeatherInfo()
        {
            InitializeComponent();
            this.Load += Form1_Load;
            comboBox1.Items.Add(MyStrings.five);
            comboBox1.Items.Add(MyStrings.ten);
            comboBox1.Items.Add(MyStrings.fifteen);
            comboBox1.SelectedItem = MyStrings.ten;
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            panelIndex++;
            if (panelIndex > 3)
            {
                panelIndex = 1;
            }
            await UpdateLabels();
        }

        private async Task<string> GetWeatherData(string city)
        {
            try
            {
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={apiKey}&units=metric";
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                return content;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"An error occurred while getting the weather data for {city}: {ex.Message}");
                return null;
            }
        }

        private async Task UpdateLabels()
        {
            switch (panelIndex)
            {
                case 1:
                    await UpdateCityWeather(MyStrings.Bengaluru);
                    break;
                case 2:
                    await UpdateCityWeather(MyStrings.Varanasi);
                    break;
                case 3:
                    await UpdateCityWeather(MyStrings.Madikeri);
                    break;
            }
        }

        private async Task UpdateCityWeather(string city)
        {
            var weatherData = await GetWeatherData(city);
            if (weatherData == null)
            {
                return;
            }
            try
            {
                dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(weatherData);
                double temperature = json.main.temp;
                double humidity = json.main.humidity;
                double pressure = json.main.pressure;

                string temperatureUnit = MyStrings.Celcius;
                if (city == MyStrings.Varanasi)
                {
                    temperature = temperature * 1.8 + 32;
                    temperatureUnit = MyStrings.Farenheit;
                }
                else if (city == MyStrings.Madikeri)
                {
                    temperature += 273.15;
                    temperatureUnit = MyStrings.Kelvin;
                    pressure /= 1000;
                }
                label1.Text = $"{city}";
                lblHumidity.Text = $"Humidity: {humidity}%";
                lblTemperature.Text = $"Temperature: {temperature}{temperatureUnit}";
                lblAtmosphere.Text = $"Atmosphere: {pressure} kPa";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while updating the weather data for {city}: {ex.Message}");
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            this.Left = WindowsFormsApp5.Properties.Settings.Default.Left;
            this.Width = WindowsFormsApp5.Properties.Settings.Default.Width;
            this.Height = WindowsFormsApp5.Properties.Settings.Default.Height;
            this.Top = WindowsFormsApp5.Properties.Settings.Default.Top;

            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            timer1.Interval = selectedInterval;
            timer1.Tick += async (s, ev) => await UpdateLabels();
            timer1.Start();
        }
   
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            WindowsFormsApp5.Properties.Settings.Default.Left = this.Left;
            WindowsFormsApp5.Properties.Settings.Default.Width = this.Width;
            WindowsFormsApp5.Properties.Settings.Default.Height = this.Height;
            WindowsFormsApp5.Properties.Settings.Default.Top = this.Top;
            WindowsFormsApp5.Properties.Settings.Default.Save();
        }
            private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    selectedInterval = 5000;
                    break;
                case 1:
                    selectedInterval = 10000;
                    break;
                case 2:
                    selectedInterval = 15000;
                    break;
            }
            timer1.Interval = selectedInterval;
        }
    }
}

