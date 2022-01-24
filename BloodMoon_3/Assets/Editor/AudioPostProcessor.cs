using System.IO;
using UnityEditor;
using UnityEngine;

public class AudioPostprocessor : AssetPostprocessor
{
    /*
     * 오디오클립 파일의 경로가 다음과 같을때
     * 
     * Sound ┬ Bgm
     *       ├ Sfx ┬ Game
     *       │     └ UI
     *       ├ Voices
     *       ├ Jingles
     *       └ Ambient
     */
    void OnPreprocessAudio()
    {
        AudioImporter audioImporter = assetImporter as AudioImporter;
        AudioImporterSampleSettings defaultSampleSettings = audioImporter.defaultSampleSettings;
        AudioImporterSampleSettings iosSampleSettings = audioImporter.GetOverrideSampleSettings("iOS");
        bool iosSettingChanged = false;

        string path = audioImporter.assetPath;

        if (path.Contains("Bgm"))
        {
            // 만약 BGM이 스테레오라면 아래 주석을 풀어준다. 그러지 않으면 실수로 다른 폴더에 넣었을다가 돌아왔을때 Force To Mono가 유지되어서 수동으로 풀어줘야함.
            //audioImporter.forceToMono = false;
            audioImporter.loadInBackground = true;

            defaultSampleSettings.loadType = AudioClipLoadType.Streaming;
            defaultSampleSettings.compressionFormat = AudioCompressionFormat.Vorbis;

            iosSampleSettings.loadType = AudioClipLoadType.Streaming;
            iosSampleSettings.compressionFormat = AudioCompressionFormat.MP3;
            iosSettingChanged = true;
        }

        if (path.Contains("Sfx"))
        {
            audioImporter.forceToMono = true;

            if (path.Contains("Game"))
            {
                // 인게임 효과음의 공통 설정
            }
            else if (path.Contains("UI"))
            {
                // UI 효과음의 공통 설정
            }
        }

        // 기타 음성, 환경음 설정도 위와 같은 식으로...

        audioImporter.defaultSampleSettings = defaultSampleSettings;
        if (iosSettingChanged)
        {
            audioImporter.SetOverrideSampleSettings("iOS", iosSampleSettings);
        }

        Debug.LogFormat("{0} has been imported to {1}.\nforce to mono - {2}\nload type - {3}\ncompression format - default: {4}, iOS: {5}",
            Path.GetFileName(path), Path.GetDirectoryName(path), audioImporter.forceToMono.ToString(), defaultSampleSettings.loadType.ToString(),
            defaultSampleSettings.compressionFormat.ToString(), iosSampleSettings.compressionFormat.ToString());
    }

    // 파일이 이동한 경우 다시 임포트.
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        foreach (string movedAsset in movedAssets)
        {
            if (movedAsset.Contains("Sound"))
            {
                AssetDatabase.ImportAsset(movedAsset);
            }
        }
    }
}
