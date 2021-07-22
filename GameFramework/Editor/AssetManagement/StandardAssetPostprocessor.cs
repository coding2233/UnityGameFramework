using OdinSerializer;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Wanderer.GameFramework
{
    public class StandardAssetPostprocessor : AssetPostprocessor
    {
        /// <summary>
        /// 在完成任意数量的资源导入后（当资源进度条到达末尾时）调用此函数。
        /// </summary>
        /// <param name="importedAssets"></param>
        /// <param name="deletedAssets"></param>
        /// <param name="movedAssets"></param>
        /// <param name="movedFromAssetPaths"></param>
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            
        }

        /// <summary>
        /// 将此函数添加到一个子类中，以在导入所有资源之前获取通知。
        /// </summary>
        void OnPreprocessAsset()
        {
            //string fileName = assetImporter.GetType().Name;
            //fileName = $"ProjectSettings/{fileName}.AssetImporter";
            //if (!File.Exists(fileName))
            //{
            //    byte[] buffer = SerializationUtility.SerializeValue(assetImporter, DataFormat.Binary);
            //    File.WriteAllBytes(fileName, buffer);
            //}
        }

        /// <summary>
        /// 将此函数添加到一个子类中，以在纹理导入器运行之前获取通知。
        /// </summary>
        void OnPreprocessTexture()
        {
            if (!CheckFlag("TextureFlag"))
            {
                TextureImporter textureImporter = (TextureImporter)assetImporter;
                textureImporter.wrapMode = TextureWrapMode.Clamp;
                textureImporter.filterMode = FilterMode.Bilinear;
                textureImporter.mipmapEnabled = false;
                textureImporter.sRGBTexture = false;
                textureImporter.isReadable = false;
                if (assetPath.StartsWith("Assets/Game/Texture/UI"))
                {
                    textureImporter.textureType = TextureImporterType.Sprite;
                }
                else
                {
                    //Android设置
                    TextureImporterPlatformSettings androidSettings = new TextureImporterPlatformSettings();
                    androidSettings.overridden = true;
                    androidSettings.name = "Android";
                    androidSettings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
                    androidSettings.format = TextureImporterFormat.ETC2_RGBA8;
                    textureImporter.SetPlatformTextureSettings(androidSettings);

                    //iOS设置
                    TextureImporterPlatformSettings iOSSettings = new TextureImporterPlatformSettings();
                    iOSSettings.overridden = true;
                    iOSSettings.name = "iOS";
                    iOSSettings.resizeAlgorithm = TextureResizeAlgorithm.Mitchell;
                    iOSSettings.format = TextureImporterFormat.PVRTC_RGBA4;
                    textureImporter.SetPlatformTextureSettings(iOSSettings);
                }
                EditorUtility.SetDirty(assetImporter);
                textureImporter.SaveAndReimport();
                AssetDatabase.Refresh();
            }
        }

        /// <summary>
        /// 将此函数添加到一个子类中，以在导入模型（.fbx、.mb 文件等）之前获取通知。
        /// </summary>
        void OnPreprocessModel()
        {
            
        }

        /// <summary>
        /// 将此函数添加到一个子类中，以在导入音频剪辑之前获取通知。
        /// </summary>
        void OnPreprocessAudio()
        {
            if (!CheckFlag("AudioFlag"))
            {
                AudioImporter audioImporter = (AudioImporter)assetImporter;
                audioImporter.forceToMono = true;
                //Android设置
                AudioImporterSampleSettings androidSettings = new AudioImporterSampleSettings();
                androidSettings.loadType = AudioClipLoadType.Streaming;
                androidSettings.compressionFormat = AudioCompressionFormat.AAC;
                androidSettings.quality = 100;
                androidSettings.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
                androidSettings.sampleRateOverride = 22050;
                audioImporter.SetOverrideSampleSettings("Android", androidSettings);
                //iOS设置
                AudioImporterSampleSettings iOSSettings = new AudioImporterSampleSettings();
                iOSSettings.loadType = AudioClipLoadType.DecompressOnLoad;
                iOSSettings.compressionFormat = AudioCompressionFormat.AAC;
                iOSSettings.quality = 100;
                iOSSettings.sampleRateSetting = AudioSampleRateSetting.OverrideSampleRate;
                iOSSettings.sampleRateOverride = 22050;
                audioImporter.SetOverrideSampleSettings("iOS", iOSSettings);

                EditorUtility.SetDirty(assetImporter);
                audioImporter.SaveAndReimport();
                AssetDatabase.Refresh();
            }
        }

        private bool CheckFlag(string key)
        {
            if (assetImporter.userData.Equals(key))
                return true;
            assetImporter.userData = key;
            return false;
        }
    }

}