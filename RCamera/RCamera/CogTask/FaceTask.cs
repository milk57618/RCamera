﻿using System.Collections.Generic;
using Android.App;
using Android.OS;
using RCamera.Model;
using System.IO;
using Com.Microsoft.Projectoxford.Face;
using GoogleGson;
using Newtonsoft.Json;
using RCamera.Helper;
using Android.Content;

/// <summary>
/// @author 강수지
/// </summary>
namespace RCamera.CogTask
{
    public class FaceTask : AsyncTask<Stream, string, string>
    {
        private ProgressDialog pd = new ProgressDialog(Application.Context);
        private const string FaceKey = "cf34900a1b6549e1882d0d9bd83dc795";
       
        public FaceServiceRestClient faceServiceClient;
        private CognitiveActivity cognitiveActivity;

        public FaceTask(CognitiveActivity cognitiveActivity)
        {
            this.cognitiveActivity = cognitiveActivity;
        }

        /// <summary>
        /// Detecting Face with Cognitive API
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        protected override string RunInBackground(params Stream[] @params)
        {
            try
            {
                PublishProgress("Detecting..."); 
                faceServiceClient = new FaceServiceRestClient(FaceKey);

                //Get Congnitive API result
                Com.Microsoft.Projectoxford.Face.Contract.Face[] result = faceServiceClient.Detect(@params[0],
                    true, //return FaceId
                    false, new FaceServiceClientFaceAttributeType[] { FaceServiceClientFaceAttributeType.Age, FaceServiceClientFaceAttributeType.Gender
                , FaceServiceClientFaceAttributeType.FacialHair
                , FaceServiceClientFaceAttributeType.Smile
                , FaceServiceClientFaceAttributeType.Glasses
                , FaceServiceClientFaceAttributeType.HeadPose});

                if (result != null)
                {
                    //parse json to String
                    Gson gson = new Gson();
                    return gson.ToJson(result);
                }
                return null;
            }
            catch (System.Exception)
            {
                PublishProgress("Detection Finished. Nothing detected");
                
                return null;
            }
        }

        protected override void OnPreExecute()
        {
            pd.Window.SetType(Android.Views.WindowManagerTypes.SystemAlert);
            pd.Show();
        }       

        protected override void OnProgressUpdate(string[] values)
        {
            pd.SetMessage(values[0]);
        }

        /// <summary>
        /// Show the result of face
        /// </summary>
        /// <param name="result"></param>
        protected override void OnPostExecute(string result)
        {
            var faces = JsonConvert.DeserializeObject<List<FaceModel>>(result);
            var bitmap = FaceFunction.DrawRectanglesOnBitmap(cognitiveActivity.mBitmap, faces);
           
            FaceFunction.setImageOutput(faces, cognitiveActivity);           
          
            cognitiveActivity.imageView.SetImageBitmap(bitmap);
            pd.Dismiss();
        }      
    }
          
}