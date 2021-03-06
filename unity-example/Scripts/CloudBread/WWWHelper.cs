﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using AssemblyCSharp;

public class WWWHelper : MonoBehaviour {

	public delegate void HttpRequestDelegate(string id, WWW www);

	public event HttpRequestDelegate OnHttpRequest;

	private int requestId;

	static WWWHelper current = null;

	static GameObject container = null;

	// Single-ton
	public static WWWHelper Instance {
		get {
			if (current == null) {
				container = new GameObject();
				container.name = "WWWHelper";
				current = container.AddComponent(typeof(WWWHelper)) as WWWHelper;
			}
			return current;
		}
	}

	public Dictionary<string, string> HeaderDic;

	public void get(string url){
		WWW www = new WWW (url);
		StartCoroutine (WaitForRequest ("1", www));

	}

	public void get(string id, string url) {
		var header = getHeaders ();
		WWW www = new WWW (url, null, header);
		
		StartCoroutine(WaitForRequest(id, www));
	}

	// POST with Dictionary JsonData
	public void POST(int id, string url, Dictionary<string, object> JsonData){
		WWWForm form = new WWWForm ();

//		if (HeaderDic == null) {
//			HeaderDic = getHeaders ();
//		}
		HeaderDic = AzureMobileAppRequestHelper.getHeader();
		foreach (KeyValuePair<string, string> post_arg in HeaderDic) {
			form.AddField(post_arg.Key, post_arg.Value);
		}

		string jsonString = JsonParser.Write (JsonData);

		// utf-8 인코딩
		byte [] bytesForEncoding = Encoding.UTF8.GetBytes ( jsonString ) ;
		string encodedString = Convert.ToBase64String (bytesForEncoding );

		// utf-8 디코딩
		byte[] decodedBytes = Convert.FromBase64String (encodedString );
		string decodedString = Encoding.UTF8.GetString (decodedBytes );

		byte[] jsonByte = Encoding.UTF8.GetBytes(decodedString);

		WWW www = new WWW(url, jsonByte, HeaderDic);
		StartCoroutine(WaitForRequest("" + id, www));

	}

	// POST with string JsonData
	public void POST(string id, string url, string JsonData){
//		WWWForm form = new WWWForm ();

//		if (HeaderDic == null) {
//			HeaderDic = getHeaders ();
//		}
		HeaderDic = AzureMobileAppRequestHelper.getHeader();

		WWW www = new WWW(url, Encoding.UTF8.GetBytes(JsonData), HeaderDic);
		StartCoroutine(WaitForRequest(id, www));
	}

	private IEnumerator  WaitForRequest(string id, WWW www) {

		yield return www;

		
		bool hasCompleteListener = (OnHttpRequest != null);

		if (hasCompleteListener) {
			OnHttpRequest(id, www);
		}
			
		www.Dispose();
	}

	private Dictionary<string, string> getHeaders(Dictionary<string, string> header){
		header ["Accept-Encoding"] = "gzip";
		header ["Accept"] = "application/json";

		header["ZUMO-API-VERSION"] = "2.0.0";
		header["X-ZUMO-VERSION"] = "ZUMO/2.0 (lang=Managed; os=Windows Store; os_version=--; arch=X86; version=2.0.31217.0)";
		header ["X-ZUMO-FEATURES"] = "AJ";
		header ["X-ZUMO-INSTALLATION-ID"] = "fe52b710-0312-4cad-8d53-dfd28d4c6f9b";
		header ["Content-Type"] = "application/json";
		header["User-Agent"] = "ZUMO/2.0 (lang=Managed; os=Windows Store; os_version=--; arch=X86; version=2.0.31217.0)";
		header ["x-zumo-auth"] = "ChangeHereForAuthentication";
		return header;
	}

	private Dictionary<string, string> getHeaders(){
		var header = new Dictionary<string, string> ();
		header ["Accept-Encoding"] = "gzip";
		header ["Accept"] = "application/json";
		
		header["ZUMO-API-VERSION"] = "2.0.0";
		header["X-ZUMO-VERSION"] = "ZUMO/2.0 (lang=Managed; os=Windows Store; os_version=--; arch=X86; version=2.0.31217.0)";
		header ["X-ZUMO-FEATURES"] = "AJ";
		header ["X-ZUMO-INSTALLATION-ID"] = "fe52b710-0312-4cad-8d53-dfd28d4c6f9b";
		header ["Content-Type"] = "application/json";
		header["User-Agent"] = "ZUMO/2.0 (lang=Managed; os=Windows Store; os_version=--; arch=X86; version=2.0.31217.0)";
		header ["x-zumo-auth"] = "ChangeHereForAuthentication";
		return header;
	}
}