﻿using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace HT.Framework
{
    [CustomEditor(typeof(ResourceManager))]
    [GithubURL("https://github.com/SaiTingHu/HTFramework")]
    [CSDNBlogURL("https://wanderer.blog.csdn.net/article/details/88852698")]
    internal sealed class ResourceManagerInspector : InternalModuleInspector<ResourceManager>
    {
        private IResourceHelper _resourceHelper;
        private string _assetBundleRootPath;
        private Dictionary<string, AssetBundle> _assetBundles;
        private AssetBundleManifest _assetBundleManifest;

        protected override string Intro
        {
            get
            {
                return "Resource Manager, Manage all resource loading and unloading!";
            }
        }

        protected override Type HelperInterface
        {
            get
            {
                return typeof(IResourceHelper);
            }
        }

        protected override void OnRuntimeEnable()
        {
            base.OnRuntimeEnable();

            _resourceHelper = Target.GetType().GetField("_helper", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(Target) as IResourceHelper;
            _assetBundleRootPath = (string)_resourceHelper.GetType().GetProperty("AssetBundleRootPath", BindingFlags.Instance | BindingFlags.Public).GetValue(_resourceHelper);
            _assetBundles = _resourceHelper.GetType().GetProperty("AssetBundles", BindingFlags.Instance | BindingFlags.Public).GetValue(_resourceHelper) as Dictionary<string, AssetBundle>;
            _assetBundleManifest = _resourceHelper.GetType().GetProperty("AssetBundleManifest", BindingFlags.Instance | BindingFlags.Public).GetValue(_resourceHelper) as AssetBundleManifest;
        }

        protected override void OnInspectorDefaultGUI()
        {
            base.OnInspectorDefaultGUI();

            GUILayout.BeginHorizontal();
            EnumPopup(Target.Mode, out Target.Mode, "Load Mode");
            GUILayout.EndHorizontal();

            if (Target.Mode == ResourceLoadMode.Resource)
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Resources Folder View", EditorGlobalTools.Styles.LargeButton))
                {
                    ResourcesFolderViewWindow window = EditorWindow.GetWindow<ResourcesFolderViewWindow>();
                    window.titleContent.image = EditorGUIUtility.IconContent("ViewToolOrbit").image;
                    window.titleContent.text = "Resources Folder View";
                    window.position = new Rect(200, 200, 400, 400);
                    window.Init();
                    window.Show();
                }
                GUILayout.EndHorizontal();
            }
            else if (Target.Mode == ResourceLoadMode.AssetBundle)
            {
                GUILayout.BeginHorizontal();
                TextField(Target.AssetBundleManifestName, out Target.AssetBundleManifestName, "Manifest Name");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                Toggle(Target.IsEditorMode, out Target.IsEditorMode, "Editor Mode");
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                Toggle(Target.IsCacheAssetBundle, out Target.IsCacheAssetBundle, "Cache AssetBundle");
                GUILayout.EndHorizontal();
            }
        }

        protected override void OnInspectorRuntimeGUI()
        {
            base.OnInspectorRuntimeGUI();

            if (Target.Mode == ResourceLoadMode.AssetBundle)
            {
                if (!Target.IsEditorMode)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Root Path: ", GUILayout.Width(100));
                    EditorGUILayout.TextField(_assetBundleRootPath);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Manifest: ", GUILayout.Width(100));
                    EditorGUILayout.ObjectField(_assetBundleManifest, typeof(AssetBundleManifest), false);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("AssetBundles: ", GUILayout.Width(100));
                    GUILayout.Label(_assetBundles.Count.ToString());
                    GUILayout.EndHorizontal();

                    foreach (KeyValuePair<string, AssetBundle> item in _assetBundles)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(20);
                        GUILayout.Label(item.Key, GUILayout.Width(80));
                        EditorGUILayout.ObjectField(item.Value, typeof(AssetBundle), false);
                        GUILayout.EndHorizontal();
                    }
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("No Runtime Data!");
                    GUILayout.EndHorizontal();
                }
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("No Runtime Data!");
                GUILayout.EndHorizontal();
            }
        }
    }
}