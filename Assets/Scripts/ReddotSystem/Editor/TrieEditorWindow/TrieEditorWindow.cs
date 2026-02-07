/*
 * Description:             TrieEditorWindow.cs
 * Author:                  TONYTANG
 * Create Date:             2022/08/12
 */

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

/// <summary>
/// TrieEditorWindow.cs
/// 前缀树窗口
/// </summary>
public class TrieEditorWindow : EditorWindow
{
    /// <summary>
    /// 居中Button GUI Style
    /// </summary>
    private GUIStyle _buttonMidStyle;

    /// <summary>
    /// 前缀树
    /// </summary>
    private Trie _trie;

    /// <summary>
    /// 当前滚动位置
    /// </summary>
    private Vector2 _currentScrollPos;

    /// <summary>
    /// 输入单词
    /// </summary>
    private string _inputWord;

    /// <summary>
    /// 节点展开Map<节点单词全名, 是否展开> 
    /// </summary>
    private Dictionary<string, bool> _trieNodeUnfoldMap = new Dictionary<string, bool>();

    /// <summary>
    /// 前缀树单词列表
    /// </summary>
    private List<string> _trieWordList;

    [MenuItem("Tools/前缀树测试窗口")]
    static void Init()
    {
        TrieEditorWindow window = (TrieEditorWindow)EditorWindow.GetWindow(typeof(TrieEditorWindow), false, "前缀树测试窗口");
        window.Show();
    }

    void OnGUI()
    {
        InitGUIStyle();
        InitData();
        _currentScrollPos = EditorGUILayout.BeginScrollView(_currentScrollPos);
        EditorGUILayout.BeginVertical();
        DisplayTrieOperationArea();
        DisplayTrieContentArea();
        DisplayTrieWordsArea();
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndScrollView();
    }

    /// <summary>
    /// 初始化GUIStyle
    /// </summary>
    private void InitGUIStyle()
    {
        if(_buttonMidStyle == null)
        {
            _buttonMidStyle = new GUIStyle("ButtonMid");
        }
    }

    /// <summary>
    /// 初始化数据
    /// </summary>
    private void InitData()
    {
        if (_trie == null)
        {
            _trie = new Trie();
            _trieWordList = null;
        }
    }

    /// <summary>
    /// 更新前缀树单词列表
    /// </summary>
    private void UpdateTrieWordList()
    {
        _trieWordList = _trie.GetWordList();
    }

    /// <summary>
    /// 显示前缀树操作区域
    /// </summary>
    private void DisplayTrieOperationArea()
    {
        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.LabelField("单词:", GUILayout.Width(40f), GUILayout.Height(20f));
        _inputWord = EditorGUILayout.TextField(_inputWord, GUILayout.ExpandWidth(true), GUILayout.Height(20f));
        if(GUILayout.Button("添加", GUILayout.Width(120f), GUILayout.Height(20f)))
        {
            if (string.IsNullOrEmpty(_inputWord))
            {
                Debug.LogError($"不能允许添加空单词!");
            }
            else
            {
                _trie.AddWord(_inputWord);
                UpdateTrieWordList();
            }
        }
        if (GUILayout.Button("删除", GUILayout.Width(120f), GUILayout.Height(20f)))
        {
            if(string.IsNullOrEmpty(_inputWord))
            {
                Debug.LogError($"不能允许删除空单词!");
            }
            else
            {
                _trie.RemoveWord(_inputWord);
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    /// <summary>
    /// 绘制前缀树内容
    /// </summary>
    private void DisplayTrieContentArea()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("前缀树节点信息", _buttonMidStyle, GUILayout.ExpandWidth(true), GUILayout.Height(20f));
        DisplayTrieNode(_trie.RootNode);
        EditorGUILayout.EndVertical();
    }

    /// <summary>
    /// 显示一个节点
    /// </summary>
    /// <param name="trieNode"></param>
    private void DisplayTrieNode(TrieNode trieNode)
    {
        var nodeFullWord = trieNode.GetFullWord();
        if(!_trieNodeUnfoldMap.ContainsKey(nodeFullWord))
        {
            _trieNodeUnfoldMap.Add(nodeFullWord, true);
        }
        EditorGUILayout.BeginHorizontal("box");
        GUILayout.Space(trieNode.Depth * 20);
        var displayName = $"{trieNode.NodeValue}({trieNode.Depth})";
        if (trieNode.ChildCount > 0)
        {
            _trieNodeUnfoldMap[nodeFullWord] = EditorGUILayout.Foldout(_trieNodeUnfoldMap[nodeFullWord], displayName);
        }
        else
        {
            EditorGUILayout.LabelField(displayName);
        }
        EditorGUILayout.EndHorizontal();
        if(_trieNodeUnfoldMap[nodeFullWord] && trieNode.ChildCount > 0)
        {
            var childNodeValueList = trieNode.ChildNodesMap.Keys.ToList();
            foreach(var childNodeValue in childNodeValueList)
            {
                var childNode = trieNode.GetChildNode(childNodeValue);
                DisplayTrieNode(childNode);
            }
        }
    }

    /// <summary>
    /// 显示前缀树单词区域
    /// </summary>
    private void DisplayTrieWordsArea()
    {
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.LabelField("前缀树单词信息", _buttonMidStyle, GUILayout.ExpandWidth(true), GUILayout.Height(20f));
        if(_trieWordList != null)
        {
            foreach (var word in _trieWordList)
            {
                EditorGUILayout.LabelField(word, GUILayout.ExpandWidth(true), GUILayout.Height(20f));
            }
        }
        EditorGUILayout.EndVertical();
    }
}
