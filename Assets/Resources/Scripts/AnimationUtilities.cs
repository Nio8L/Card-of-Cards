using System.Collections;
using System.Collections.Generic;
using Mono.Cecil;
using UnityEditor;
using UnityEditor.Tilemaps;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AnimationUtilities : MonoBehaviour{
    public static AnimationUtilities animationUtilities;

    List<AnimationInstance> allAnimations = new List<AnimationInstance>();
    void Start(){
        // Destroy this object if an AnimationUtilities manager already exists
        if(animationUtilities != null){
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        animationUtilities = this;
    }

    void Update(){
        for (int i = 0; i < allAnimations.Count; i++){
            AnimationInstance animationInstance = allAnimations[i];
            if (animationInstance.Call()){
                allAnimations.RemoveAt(i);
                i--;
            }
        }
    }

    class AnimationInstance{
        // Class that hold the information for animations
        Transform target;
        float timeLeft;
        float startDelay;
        float totalTime;
        string animation;
        List<Vector3> points = new List<Vector3>();
        List<float> values = new List<float>();
        
        List<bool> bools = new List<bool>();
        public AnimationInstance(Transform _target, float _timeLeft, string _animation){
            // Constructor
            target    = _target   ;
            timeLeft  = _timeLeft ;
            totalTime = _timeLeft;
            animation = _animation;
        }
        public void AddPoint(Vector3 position){
            // Adds Vector3 to a list
            points.Add(position);
        }
        public void AddValue(float value){
            // Adds floats to a list
            values.Add(value);
        }
        public void SetDelay(float _startDelay){
            // Set a start delay
            startDelay = _startDelay;
        }
        public bool Call(){
            // Called every frame from Update

            // If the target object no longer exist return true removing this animation from the list
            if (target == null) return true;

            // Apply delay
            if (startDelay > 0){
                startDelay -= Time.deltaTime;
                return false;
            }

            // Trigger correct function
            timeLeft -= Time.deltaTime;
            if (animation == "MoveToPoint"){
                MoveToPointInst();
            }else if (animation == "ChangeAlpha"){
                ChangeAlphaInst();
            }else if (animation == "ReturnMoveToPoint"){
                ReturnMoveToPointInst();
            }else if (animation == "ChangeCanvasAlpha"){
                ChangeCanvasAlphaInst();
            }else if (animation == "DestroyAfter"){
                DestroyAfterInst();
            }

            // Return true if the animation is complete
            if (timeLeft <= 0) return true;
            return false;
        }
        void MoveToPointInst(){
            // Move an object to a certain point
            Vector3 newPosition = Vector3.Lerp(points[0], points[1], 1 - timeLeft/totalTime);
            target.transform.position = newPosition;
        } 
        void ChangeAlphaInst(){
            // Change the alpha of an object
            float alpha = Mathf.Lerp(values[0], values[1], 1 - timeLeft/totalTime);

            Image image = target.GetComponent<Image>();
            if (image != null){
                image.color = new Color(image.color.r, image.color.g, image.color.b, alpha);
            }else{
                SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
                spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);
            }
        }

        void ChangeCanvasAlphaInst(){
            // Change the alpha of a canvas
            float alpha = Mathf.Lerp(values[0], values[1], 1 - timeLeft / totalTime);

            CanvasGroup canvas = target.GetComponent<CanvasGroup>();
            if(canvas != null){
                canvas.alpha = alpha;
            }
        }

        void ReturnMoveToPointInst(){
            // Move to a point, then return
            Vector3 newPosition = Vector3.Lerp(points[0], points[1], 1 - timeLeft/totalTime);
            target.transform.position = newPosition;

            if (timeLeft <= 0 && bools.Count == 0){
                timeLeft = totalTime;
                Vector3 swap = points[0];
                points[0] = points[1];
                points[1] = swap;

                bools.Add(true);
            }
        }

        void DestroyAfterInst(){
            // Destroy a target
            Destroy(target);
        }
        public Transform GetTarget(){
            // Return this animationInstances target
            return target;
        }
    }
    public static bool CheckForAnimation(GameObject gameObject){
        // Returns if the given GameObject has active animations
        for (int i = 0; i < animationUtilities.allAnimations.Count; i++){
            AnimationInstance animationInstance = animationUtilities.allAnimations[i];
            if (animationInstance.GetTarget() == gameObject.transform){
                return true;
            }
        }
        return false;
    }
    public static void MoveToPoint(Transform target, float time, float delay, Vector3 point){
        // Move a given target to a point in a certain time after a delay
        AnimationInstance newAnimation = new AnimationInstance(target, time, "MoveToPoint");
        newAnimation.SetDelay(delay);

        newAnimation.AddPoint(target.transform.position);
        newAnimation.AddPoint(point);
        animationUtilities.allAnimations.Add(newAnimation);
    }
    public static void ReturnMoveToPoint(Transform target, float time, float delay, Vector3 point){
        // Move a given target to a point in a certain time after a delay and then return back
        AnimationInstance newAnimation = new AnimationInstance(target, time, "ReturnMoveToPoint");
        newAnimation.SetDelay(delay);

        newAnimation.AddPoint(target.transform.position);
        newAnimation.AddPoint(point);
        animationUtilities.allAnimations.Add(newAnimation);
    }
    public static void ChangeAlpha(Transform target, float time, float delay, float alpha){
        // Change the alpha of an object
        AnimationInstance newAnimation = new AnimationInstance(target, time, "ChangeAlpha");
        newAnimation.SetDelay(delay);

        SpriteRenderer spriteRenderer = target.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null){
            newAnimation.AddValue(spriteRenderer.color.a);
        }else{
            newAnimation.AddValue(target.GetComponent<Image>().color.a);
        }

        newAnimation.AddValue(alpha);

        animationUtilities.allAnimations.Add(newAnimation);
    }
    public static void ChangeCanvasAlpha(Transform target, float time, float delay, float alpha){
        // Change the alpha of a canvas
        AnimationInstance newAnimation = new AnimationInstance(target, time, "ChangeCanvasAlpha");
        newAnimation.SetDelay(delay);

        CanvasGroup canvas = target.GetComponent<CanvasGroup>();
        if(canvas != null){
            newAnimation.AddValue(canvas.alpha);
        }

        newAnimation.AddValue(alpha);

        animationUtilities.allAnimations.Add(newAnimation);
    }
    public static void DestroyAfter(Transform target, float delay){
        // Start a destroy timer
        AnimationInstance newAnimation = new AnimationInstance(target, 0, "DestroyAfter");
        newAnimation.SetDelay(delay);
        animationUtilities.allAnimations.Add(newAnimation);
    }


}
    
