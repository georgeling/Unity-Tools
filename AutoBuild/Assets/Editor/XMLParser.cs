using UnityEngine;
using System.Collections;
using System.IO;
using System.Xml;

/// <summary>
/// XML parser, 效率比较低, 适合配置简单的xml
/// 默认标签<document>
/// </summary>
public class XMLParser {

	private XmlDocument xmlDoc = null;
	private XmlNode nodeDoc = null;

	public static XMLParser Create(string path){
		XMLParser xmlParserObj = null; 
		if (File.Exists (path)) {
			xmlParserObj = new XMLParser();
			xmlParserObj.LoadXml(path);
		} else {
			Debug.LogError(path + " is not exist!!");
		}

		return xmlParserObj;
	}

	private void LoadXml(string path){
		xmlDoc = new XmlDocument ();
		xmlDoc.Load (path);
		nodeDoc = xmlDoc.SelectSingleNode("document");
	}

	public XmlNode GetSingleNode(string tag){
		XmlNode node = null;
		if (xmlDoc != null) {
			try{
				foreach(XmlNode data in nodeDoc.ChildNodes){
					if(data.Name.Equals(tag)){
						node = data;
						break;
					}
				}
			}catch(IOException e){
				Debug.Log("Get " + tag + "error: " + e.Message);
			}
		} else {
			Debug.Log(xmlDoc.Name + "is not Loaded!!");
		}

		return node;
	}
}
