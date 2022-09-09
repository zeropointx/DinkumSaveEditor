using Microsoft.Win32;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace SaveGameEditor
{
    public class SaveLoader
    {
		public bool IsDllLoaded()
        {
			return Type.GetType(saveTypeCache[@"/photoDetails.dat"]) != null;
        }
		internal class AssemblyResolver
		{
			public static void Register()
			{
				AppDomain.CurrentDomain.AssemblyResolve +=
				  (sender, args) =>
				  {
					  var an = new AssemblyName(args.Name);
					  if (an.Name == "Assembly-CSharp")
					  {
						  string resourcepath = "Assembly-CSharp.dll";

						  using (FileStream stream = File.OpenRead(getDllPath()+resourcepath))
						  {
							  using (stream)
							  {
								  byte[] data = new byte[stream.Length];
								  stream.Read(data, 0, data.Length);
								  stream.Close();
								  return Assembly.Load(data);
							  }
						  }
					  }
					  return null;
				  };

			}
		}

        public void transferActiveSave()
        {
			string item = (string)Form1.form1.comboBox1.SelectedItem;
			if (item == null)
				return;
			int index = int.Parse(item.Replace("Slot", ""));
			string newPath = getRePackagePath(index);
			foreach (string s in getSaveSlots())
			{
				if (s.Contains(item))
                {
					var files = Directory.GetFiles(newPath);
					foreach(var file in files)
                    {
						string path = s +"\\"+ Path.GetFileName(file);
						File.Copy(file, path, true);
                    }
					break;
                }
			}
		}

        public void copyGameAssemblies()
        {
			string path = getDllPath();
			var files = Directory.GetFiles(path);
			foreach(var f in files)
            {
				var localPath = ".\\" + Path.GetFileName(f);
				if (Path.GetFileName(f) != "Assembly-CSharp.dll" && !File.Exists(localPath))
				File.Copy(f, localPath);
            }
        }
		public static string getDllPath()
		{
			var reg = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Valve\Steam", "InstallPath", null);
			var reg32 = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Valve\Steam", "InstallPath", null);
			string installPath = (string)(reg != null ? reg : reg32);
			Console.Out.WriteLine(installPath);
			string libraryPath = $@"{installPath}\steamapps\libraryfolders.vdf";
			Console.Out.WriteLine(libraryPath);
			string value = File.ReadAllText(libraryPath);
			var matches = Regex.Matches(value, "\"path\"(.*)\"(.*)\"");
			foreach (Match match in matches)
			{
				var g = (string)match.Groups[2].Value;
				var appsPath = $@"{g}\steamapps\common\Dinkum";
				Console.Out.WriteLine(appsPath);
				bool exists = Directory.Exists(appsPath);
				if (!exists)
					continue;
				Console.Out.WriteLine("Exists?: " + exists);
				string fullPath = $@"{appsPath}\Dinkum_Data\Managed\";
				bool exists2 = Directory.Exists(fullPath);
				if (!exists2)
					continue;
				Console.Out.WriteLine(fullPath);
				return fullPath;
			}
			return null;
		}
		public SaveLoader()
        {
			if (!IsDllLoaded())
			{
				AssemblyResolver.Register();
				bool loaded = IsDllLoaded();
				createAssembly();
				MessageBox.Show("This app will auto-restart now. (First time only)");
				System.Windows.Forms.Application.Restart();
				Environment.Exit(0);
			}
			foreach(string s in getSaveSlots())
            {
				Form1.form1.comboBox1.Items.Add(Path.GetFileName(s));
			}
			Form1.form1.comboBox1.SelectedIndex = 0;
		}
		public string[] getSaveSlots()
        {
			var fullPath = getSaveRoot();
			var directories = Directory.GetDirectories(fullPath);
			return directories;
		}
		string savePath = "";
		public string saveSlot()
        {
			return savePath;
        }
		string getSaveRoot()
        {
			string userPath = System.Environment.GetEnvironmentVariable("USERPROFILE");
			string fullpath = $@"{userPath}\Appdata\LocalLow\James Bendon\Dinkum\";
			return fullpath;
		}
		public int saveIndex = 0;
		public void setSaveSlot(int slot)
        {
			savePath = getSaveRoot() + "\\Slot" + slot;
			saveIndex = slot;
		}
		public string photoGraphedType = "PhotographedObject, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null";
	
		public string[] saveDatas = new string[] { "/photoDetails.dat", "/vehicleInfo.dat", "/licences.dat", "/quests.dat",
		"/date.dat",  "/townSave.dat", "/onTop.dat", "/npc.dat", "/unlocked.dat", "/deeds.dat", "/pedia.dat", "/townStatus.dat", "/changers.dat",  "/levels.dat",
		 "/museumSave.dat","/bboard.dat", "/mail.dat", "/houseSave.dat", "/mapIcons.dat", "/drops.dat", "/carry.dat", "/animalDetails.dat", "/playerInfo.dat", "/farmAnimalSave.dat",
		"/animalHouseSave.dat"};
		Dictionary<string, object> saveCache = new Dictionary<string, object>();
		Dictionary<string, string> saveTypeCache = new Dictionary<string, string>()
		{
			
			{ @"/photoDetails.dat","PhotoSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
			{ "/vehicleInfo.dat","VehicleSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/licences.dat","LicenceAndPermitPointSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/quests.dat","QuestSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/date.dat","DateSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/townSave.dat","TownManagerSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/onTop.dat","ItemOnTopSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/npc.dat","NPCsave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/unlocked.dat","RecipesUnlockedSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/deeds.dat","DeedSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/pedia.dat","PediaSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/townStatus.dat","TownStatusSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/changers.dat","ChangerSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/levels.dat","LevelSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/museumSave.dat","MuseumSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/bboard.dat","BulletinBoardSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/mail.dat","MailSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/houseSave.dat","HouseListSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/mapIcons.dat","MapIconSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/drops.dat","DropSaves, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/carry.dat","CarrySave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/animalDetails.dat","FencedOffAnimalSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{ @"/playerInfo.dat","PlayerInv, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{@"/farmAnimalSave.dat",  "FarmAnimalSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null"},
{@"/animalHouseSave.dat", "AnimalHouseSave, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null" }
		};
		public string newLoc = @".\DecryptedSaves\";
		public void createAssembly()
        {
			var slot = getSaveSlots()[0];
			copyGameAssemblies();
			setSaveSlot(int.Parse(slot.Remove(0, slot.IndexOf("Slot")).Replace(".dat", "").Replace("Slot", "")));

			AssemblyDefinition adef = AssemblyDefinition.ReadAssembly(getDllPath()+ "Assembly-CSharp.dll");

			foreach (string s in saveDatas)
			{
				if (File.Exists(saveSlot() + s))
				{
					try
					{
						var name = saveTypeCache[s].Split(',');
						TypeDefinition tdef = adef.MainModule.GetType(name[0]);
						tdef.IsPublic = true;
					}
					catch (Exception ex)
					{
						Console.Out.WriteLine($"error loading {s}" + ex);
					}
				}
			}
			TypeDefinition def2 = adef.MainModule.GetType(Type.GetType(photoGraphedType).Name);
			
			AddEmptyConstructor(def2);
			adef.MainModule.Write(@".\Assembly-CSharp.dll");
		}

		void AddEmptyConstructor(TypeDefinition type)
		{
			var methodAttributes = Mono.Cecil.MethodAttributes.Public | Mono.Cecil.MethodAttributes.HideBySig | Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName;
			var method = new MethodDefinition(".ctor", methodAttributes, type.Module.TypeSystem.Void);
			var methodReference = new MethodReference(".ctor", type.Module.TypeSystem.Void, type.BaseType) { HasThis = true };
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Call, methodReference));
			method.Body.Instructions.Add(Instruction.Create(OpCodes.Ret));
			type.Methods.Add(method);
		}
		public string getRePackagePath(int index)
        {
			return $@".\RePackaged\Slot{saveIndex}";
		}
		public void saveAll()
        {
			foreach (var slot in getSaveSlots())
			{
				saveIndex = int.Parse(Path.GetFileName(slot).Replace(".dat", "").Replace("Slot", ""));
				savePath = getRePackagePath(saveIndex);
				foreach (string s in saveDatas)
				{
					FileStream fileStream = null;
					try
					{
						BinaryFormatter binaryFormatter = new BinaryFormatter();
						Directory.CreateDirectory(Path.GetDirectoryName(saveSlot() + s));
						fileStream = File.Create(saveSlot() + s);

						string path = $@"{newLoc}\Slot{saveIndex}\{s.Replace(".dat", ".json")}";
						var obj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText(path), Type.GetType(saveTypeCache[s]));
						binaryFormatter.Serialize(fileStream, obj);
						fileStream.Close();
					}
					catch (Exception ex)
					{
						Console.Out.WriteLine($"error loading {s}" + ex);
						fileStream?.Close();
					}
				}
			}
		}

		void recursiveAddFiles(string directory, ZipArchive archive, string rootPath)
        {
			foreach(var dir in Directory.GetDirectories(directory))
            {
				var fullName = dir.Replace(rootPath, "").Replace("\\", "/");
				fullName += "/";
				var entry = archive.CreateEntry(fullName);
				entry.LastWriteTime = Directory.GetLastWriteTime(dir);
				recursiveAddFiles(dir, archive, rootPath);

			}
			foreach (var file in Directory.GetFiles(directory))
			{
				var fullName = file.Replace(rootPath, "").Replace("\\","/");
				var entryName = Path.GetFileName(file);
				var entry = archive.CreateEntry(fullName);
				entry.LastWriteTime = File.GetLastWriteTime(file);
				using (var fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				using (var stream = entry.Open())
				{
					fs.CopyTo(stream);
				}
			}
		}

		void SafelyCreateZipFromDirectory(string sourceDirectoryName, string zipFilePath)
		{
			using (FileStream zipToOpen = new FileStream(zipFilePath, FileMode.Create))
			using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
			{
				recursiveAddFiles(sourceDirectoryName, archive,sourceDirectoryName);
			}
		}

		public void loadAll()
        {
			string filePath = "SaveBackup_"+DateTime.Now.ToString() + ".zip";
			string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
			filePath = filePath.Replace(' ', '_');
			foreach (char c in invalid)
			{
				filePath = filePath.Replace(c.ToString(), "_");
			}
			filePath = newLoc + filePath;
			Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			SafelyCreateZipFromDirectory(getSaveRoot(), filePath);
			foreach (var slot in getSaveSlots())
			{
				setSaveSlot(int.Parse(slot.Remove(0,slot.IndexOf("Slot")).Replace(".dat", "").Replace("Slot","")));
				foreach (string s in saveDatas)
				{
					if (File.Exists(saveSlot() + s))
					{
						FileStream fileStream = null;
						try
						{
							BinaryFormatter binaryFormatter = new BinaryFormatter();
							fileStream = File.Open(saveSlot() + s, FileMode.Open);
							object obj = Utility.load(fileStream);
							saveCache.Add(s, obj);
							fileStream.Close();

							var json = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
							string path = $@"{newLoc}\Slot{saveIndex}\{s.Replace(".dat", ".json")}";
							Directory.CreateDirectory(Path.GetDirectoryName(path));
							File.WriteAllText(path, json);
							if (!saveTypeCache.ContainsKey(s))
							saveTypeCache.Add(s, obj.GetType().AssemblyQualifiedName);
						}
						catch (Exception ex)
						{
							Console.Out.WriteLine($"error loading {s}" + ex);
							fileStream?.Close();
						}
					}
				}
				saveCache.Clear();
			}
        }


    }
}
