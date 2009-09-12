﻿using System;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Text.RegularExpressions;

namespace XviD4PSP
{
	public partial class ColorCorrection
	{
        public Massive m;
        private Massive oldm;
        private MainWindow p;

        public ColorCorrection(Massive mass, MainWindow parent)
		{
			this.InitializeComponent();

            m = mass.Clone();
            oldm = mass.Clone();
            p = parent;
            Owner = m.owner;

            //переводим
            Title = Languages.Translate("Color correction");
            text_profile.Content = Languages.Translate("Profile:");
            button_apply.Content = Languages.Translate("Apply");
            button_apply.ToolTip = Languages.Translate("Refresh preview");
            button_cancel.Content = Languages.Translate("Cancel");
            button_ok.Content = Languages.Translate("OK");
            button_add.ToolTip = Languages.Translate("Add profile");
            button_remove.ToolTip = Languages.Translate("Remove profile");
            text_brightness.Content = Languages.Translate("Brightness") + ":";
            text_saturation.Content = Languages.Translate("Saturation") + ":";
            text_contrast.Content = Languages.Translate("Contrast") + ":";
            text_hue.Content = Languages.Translate("Hue") + ":";
            text_histogram.Content = Languages.Translate("Histogram") + ":";

            combo_brightness.ToolTip = Languages.Translate("Is used to change the brightness of the image.") + Environment.NewLine +
               Languages.Translate("Positive values increase the brightness.") + Environment.NewLine + 
              Languages.Translate("Negative values decrease the brightness.");

            combo_hue.ToolTip = Languages.Translate("Is used to adjust the color hue of the image.") + Environment.NewLine +
                Languages.Translate("Positive values shift the image towards red.") + Environment.NewLine +
                Languages.Translate("Negative values shift it towards green.");

            combo_contrast.ToolTip = Languages.Translate("Is used to change the contrast of the image.") + Environment.NewLine +
                Languages.Translate("Values above 1.0 increase the contrast.") + Environment.NewLine +
                Languages.Translate("Values below 1.0 decrease the contrast.");

            combo_saturation.ToolTip = Languages.Translate("Is used to adjust the color saturation of the image.") + Environment.NewLine +
                Languages.Translate("Values above 1.0 increase the saturation.") + Environment.NewLine +
                Languages.Translate("Values below 1.0 reduce the saturation.");

            //забиваем параметры
            for (double n = 0.0; n <= 10.0; n += 0.1) //цветность
                combo_saturation.Items.Add(n.ToString("0.0").Replace(",", "."));
            slider_saturation.Minimum = 0.0;
            slider_saturation.Maximum = 10.0;
            slider_saturation.SmallChange = 0.1;


            for (double n = 0.0; n <= 5.0; n += 0.01) //(double n = 0.0; n <= 10.0; n += 0.1)
                combo_contrast.Items.Add(n.ToString("0.00").Replace(",", "."));//Контрастность //("0.0")
            slider_contrast.Minimum = 0.0;
            slider_contrast.Maximum = 5.0; //10.0
            slider_contrast.SmallChange = 0.01; //0.1

            
            for (int n = -180; n <= 180; n++) //Оттенок
                combo_hue.Items.Add(n);
            slider_hue.Minimum = -180;
            slider_hue.Maximum = 180;
            slider_saturation.SmallChange = 1; //<-----

  
            for (int n = -255; n <= 255; n++) //Яркость
                combo_brightness.Items.Add(n);
            slider_brightness.Minimum = -255;
            slider_brightness.Maximum = 255;
            slider_brightness.SmallChange = 1;

            //возможные типы гистограммы
            combo_histogram.Items.Add("Disabled");
            combo_histogram.Items.Add("Classic");
            combo_histogram.Items.Add("Levels");
            combo_histogram.Items.Add("Color");
            combo_histogram.Items.Add("Color2");
            combo_histogram.Items.Add("Luma");
            combo_histogram.Items.Add("Stereo");
            combo_histogram.Items.Add("StereoOverlay");
            combo_histogram.Items.Add("AudioLevels");
            combo_histogram.SelectedItem = oldm.levels;

            LoadProfiles();//загружает список профилей в форму, название текущего профиля выбирается = m.sbc
            DecodeProfile(m);//читает и передает массиву mass значения параметров из файла профиля = m.sbc
            LoadFromProfile();//загружает эти параметры в форму (из массива m)

            ShowDialog();
		}

        private void button_ok_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Close();
        }

        private void button_cancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            m = oldm.Clone();
            Close();
        }

        private void combo_profile_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (combo_profile.IsDropDownOpen || combo_profile.IsSelectionBoxHighlighted)
            {
                m.sbc = combo_profile.SelectedItem.ToString();

                DecodeProfile(m);//читает и передает массиву mass значения параметров из файла профиля = m.sbc
                LoadFromProfile();//загружает значения параметров в форму (из массива m)
                Refresh();
            }
        }

        private void LoadProfiles() //загружает список профилей в форму, текущий профиль выбирается = m.sbc
        {
            //загружаем списки профилей цвето коррекции
            combo_profile.Items.Clear();
            combo_profile.Items.Add("Disabled");
            foreach (string file in Directory.GetFiles(Calculate.StartupPath + "\\presets\\sbc"))
            {
                string name = Path.GetFileNameWithoutExtension(file);
                combo_profile.Items.Add(name);
            }
            //прописываем текущий профиль
            if (combo_profile.Items.Contains(m.sbc))
                combo_profile.SelectedItem = m.sbc;
            else
                combo_profile.SelectedItem = "Disabled";
        }

        public static Massive DecodeProfile(Massive mass)//читает и передает массиву mass значения параметров из файла профиля = m.sbc
          {
            //обнуляем параметры на параметры по умолчанию
            mass.iscolormatrix = false;
            mass.saturation = 1.0;
            mass.brightness = 0;
            mass.contrast = 1.00; //1.0
            mass.hue = 0;
            mass.levels = "Disabled";//режим гистограммы

            if (mass.sbc == "Disabled")
                return mass;

            string line;
            using (StreamReader sr = new StreamReader(Calculate.StartupPath + 
                "\\presets\\sbc\\" + mass.sbc + ".avs", System.Text.Encoding.Default)) //ищет и читает файл с пресетами цветокоррекции
            {
                while (!sr.EndOfStream)
                {
                    line = sr.ReadLine();
                    //дешифровка яркости контраста ...
                    if (line.ToLower().StartsWith("tweak")) //начинает читать строчку со слова tweak
                    {
                        string pat;
                        Regex r;
                        Match mat;

                        //получаем hue - оттенок
                        pat = @"hue=(\d+)";
                        r = new Regex(pat, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
                        mat = r.Match(line);
                        if (mat.Success)
                            mass.hue = Convert.ToInt32(mat.Groups[1].Value);

                        //получаем hue-
                        pat = @"hue=-(\d+)";
                        r = new Regex(pat, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
                        mat = r.Match(line);
                        if (mat.Success)
                            mass.hue = Convert.ToInt32("-" + mat.Groups[1].Value.ToString());

                        //получаем насыщенность
                        pat = @"sat=(\d+.\d+)";
                        r = new Regex(pat, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
                        mat = r.Match(line);
                        if (mat.Success)
                            mass.saturation = Convert.ToDouble(Calculate.ConvertStringToDouble(mat.Groups[1].Value));

                        //получаем яркость
                        pat = @"bright=(\d+)";
                        r = new Regex(pat, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
                        mat = r.Match(line);
                        if (mat.Success)
                            mass.brightness = Convert.ToInt32(mat.Groups[1].Value);

                        //получаем яркость
                        pat = @"bright=-(\d+)";
                        r = new Regex(pat, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
                        mat = r.Match(line);
                        if (mat.Success)
                            mass.brightness = Convert.ToInt32("-" + mat.Groups[1].Value);

                        //получаем контраст
                        pat = @"cont=(\d+.\d+)";
                        r = new Regex(pat, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
                        mat = r.Match(line);
                        if (mat.Success)
                            mass.contrast = Convert.ToDouble(Calculate.ConvertStringToDouble(mat.Groups[1].Value));
                    }

                    //дешифровка ColorMatrix
                    if (line.ToLower() == "colormatrix()")
                        mass.iscolormatrix = true;
                }
            }

            return mass;
        }

        private void LoadFromProfile() //загружает значения параметров в форму (из массива m)
        {
            check_colormatrix.IsChecked = m.iscolormatrix;
            combo_saturation.SelectedItem = m.saturation.ToString("0.0").Replace(",", ".");
            combo_hue.SelectedItem = m.hue;
            combo_brightness.SelectedItem = m.brightness;
            combo_contrast.SelectedItem = m.contrast.ToString("0.00").Replace(",", "."); //("0.0")

            slider_saturation.Value = m.saturation;
            slider_hue.Value = m.hue;
            slider_brightness.Value = m.brightness;
            slider_contrast.Value = m.contrast;
        }

        private void button_add_Click(object sender, System.Windows.RoutedEventArgs e) //кнопка "добавить новый профиль"
        {
            string auto_name = "Custom";
            if (m.iscolormatrix)
                auto_name += " DVD";


            NewProfile newp = new NewProfile(auto_name, Format.EnumToString(m.format), NewProfile.ProfileType.SBC, this); //создается новый профиль

            if (newp.profile != null)
            {
                m.sbc = newp.profile;
                CreateSBCProfile();//
                LoadProfiles();//загружает список профилей в форму, название текущего профиля выбирается = m.sbc
            }
        }

        private void button_remove_Click(object sender, System.Windows.RoutedEventArgs e) //кнопка "удалить профиль"
        {
            if (combo_profile.Items.Count > 1)
            {
                Message mess = new Message(this);
                mess.ShowMessage(Languages.Translate("Do you realy want remove profile") + " " + m.sbc + "?",
                    Languages.Translate("Question"),
                    Message.MessageStyle.YesNo);

                if (mess.result == Message.Result.Yes)
                {
                    int last_num = combo_profile.SelectedIndex;
                    string profile_path = Calculate.StartupPath + "\\presets\\sbc\\" + m.sbc + ".avs";
                    File.Delete(profile_path);

                    //загружаем список фильтров
                    combo_profile.Items.Clear();
                    foreach (string file in Directory.GetFiles(Calculate.StartupPath + "\\presets\\sbc"))
                    {
                        string name = Path.GetFileNameWithoutExtension(file);
                        combo_profile.Items.Add(name);
                    }

                    //прописываем текущий пресет кодирования
                    if (last_num == 0)
                        m.sbc = combo_profile.Items[0].ToString();
                    else
                        m.sbc = combo_profile.Items[last_num - 1].ToString();
                    combo_profile.SelectedItem = m.sbc;

                    combo_profile.UpdateLayout();

                    DecodeProfile(m);
                    LoadFromProfile();
                    Refresh();
                }
            }
            else
            {
                Message mess = new Message(this);
                mess.ShowMessage(Languages.Translate("Not allowed remove last profile!"),
                    Languages.Translate("Warning"),
                    Message.MessageStyle.Ok);
            }
        }

        private void UpdateManualProfile() //изменяет название текущего профиля на Manual и сохраняет его в виде файла
        {
            m.sbc = "Manual";
            CreateSBCProfile();
            LoadProfiles();
        }

        private void CreateSBCProfile() //создает новый пресет-файл
        {
            StreamWriter sw = new StreamWriter(Calculate.StartupPath + "\\presets\\sbc\\" + m.sbc + ".avs", false, System.Text.Encoding.Default);

            //каждый параметр будет записываться на новой строчке, через одну
            if (m.iscolormatrix)
                sw.WriteLine("ColorMatrix()" + Environment.NewLine);

            if (m.hue != 0)
                sw.WriteLine("Tweak(hue=" + m.hue + ")" + Environment.NewLine);

            if (m.saturation != 1.0)
                sw.WriteLine("Tweak(sat=" + m.saturation.ToString("0.0").Replace(",", ".") + ")" + Environment.NewLine);

            if (m.brightness != 0)
                sw.WriteLine("Tweak(bright=" + m.brightness + ")" + Environment.NewLine);

            if (m.contrast != 1.0)
                sw.WriteLine("Tweak(cont=" + m.contrast.ToString("0.00").Replace(",", ".") + ")" + Environment.NewLine); //("0.0")

            sw.Close();
        }

        private void slider_saturation_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (slider_saturation.IsFocused)
            {
                m.saturation = slider_saturation.Value;
                combo_saturation.SelectedItem = m.saturation.ToString("0.0").Replace(",", ".");
                UpdateManualProfile();
            }
        }

        private void combo_saturation_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (combo_saturation.IsDropDownOpen || combo_saturation.IsSelectionBoxHighlighted)
            {
                m.saturation = Calculate.ConvertStringToDouble(combo_saturation.SelectedItem.ToString());
                slider_saturation.Value = m.saturation;
                UpdateManualProfile();
                Refresh();
            }
        }

        private void slider_hue_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (slider_hue.IsFocused)
            {
                m.hue = Convert.ToInt32(slider_hue.Value);
                combo_hue.SelectedItem = m.hue;
                UpdateManualProfile();
            }
        }

        private void combo_hue_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (combo_hue.IsDropDownOpen || combo_hue.IsSelectionBoxHighlighted)
            {
                m.hue = Convert.ToInt32(combo_hue.SelectedItem);
                slider_hue.Value = m.hue;
                UpdateManualProfile();
                Refresh();
            }
        }

        private void slider_contrast_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (slider_contrast.IsFocused)
            {
                m.contrast = slider_contrast.Value;
                combo_contrast.SelectedItem = m.contrast.ToString("0.00").Replace(",", "."); //("0.00")
                UpdateManualProfile();
            }
        }

        private void combo_contrast_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (combo_contrast.IsDropDownOpen || combo_contrast.IsSelectionBoxHighlighted)
            {
                m.contrast = Calculate.ConvertStringToDouble(combo_contrast.SelectedItem.ToString());
                slider_contrast.Value = m.contrast;
                UpdateManualProfile();
                Refresh();
            }
        }

        private void slider_brightness_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            if (slider_brightness.IsFocused)
            {
                m.brightness = Convert.ToInt32(slider_brightness.Value);
                combo_brightness.SelectedItem = m.brightness;
                UpdateManualProfile();
            }
        }

        private void combo_brightness_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (combo_brightness.IsDropDownOpen || combo_brightness.IsSelectionBoxHighlighted)
            {
                m.brightness = Convert.ToInt32(combo_brightness.SelectedItem);
                slider_brightness.Value = m.brightness;
                UpdateManualProfile();
                Refresh();
            }
        }

        private void check_colormatrix_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (check_colormatrix.IsFocused)
            {
                m.iscolormatrix = check_colormatrix.IsChecked.Value;
                UpdateManualProfile();
                Refresh();
            }
        }

        private void check_colormatrix_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            if (check_colormatrix.IsFocused)
            {
                m.iscolormatrix = check_colormatrix.IsChecked.Value;
                UpdateManualProfile();
                Refresh();
            }
        }

        private void Refresh()
        {
            m = AviSynthScripting.CreateAutoAviSynthScript(m);
            p.m = m.Clone();
            p.Refresh(m.script);
        }

        private void button_apply_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Refresh();
        }

        private void button_fullscreen_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            p.SwitchToFullScreen();        
        }

        //Обработка выбора режима отображения гистограммы
        private void combo_histogram_SelectionChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            if (combo_histogram.IsDropDownOpen || combo_histogram.IsSelectionBoxHighlighted)
            {
                m.levels = Convert.ToString(combo_histogram.SelectedItem);
                Refresh();

            }

        }

	}
}