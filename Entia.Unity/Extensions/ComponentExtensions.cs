using Entia.Injectables;
using Entia.Unity.Components;
using Entia.Unity.Mappers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Animations;
using UnityEngine.EventSystems;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Entia.Unity
{
    public static class ComponentExtensions
    {
        public static bool TryUnity<T>(this Modules.Components components, Entity entity, out T @object) where T : Object
        {
            if (components.TryRead<Unity<T>>(entity, out var read))
            {
                @object = read.Value.Value;
                return true;
            }

            @object = default;
            return false;
        }

        public static bool TryUnity<T>(this AllComponents components, Entity entity, out T @object) where T : Object
        {
            if (components.TryRead<Unity<T>>(entity, out var read))
            {
                @object = read.Value.Value;
                return true;
            }

            @object = default;
            return false;
        }

        public static T Unity<T>(this Modules.Components components, Entity entity) where T : Object => components.Read<Unity<T>>(entity).Value;

        public static T Unity<T>(this AllComponents components, Entity entity) where T : Object => components.Read<Unity<T>>(entity).Value;

        public static TOut Map<TMap, TOut>(this Component component, TMap mapper) where TMap : IMapper<Component, TOut>
        {
            switch (component)
            {
                case StandaloneInputModule standaloneInputModule: return mapper.Map(standaloneInputModule);
                case Physics2DRaycaster physics2DRaycaster: return mapper.Map(physics2DRaycaster);
                case Image image: return mapper.Map(image);
                case RawImage rawImage: return mapper.Map(rawImage);
                case Text text: return mapper.Map(text);
                case HorizontalLayoutGroup horizontalLayoutGroup: return mapper.Map(horizontalLayoutGroup);
                case VerticalLayoutGroup verticalLayoutGroup: return mapper.Map(verticalLayoutGroup);
                case Outline outline: return mapper.Map(outline);
                case PhysicsRaycaster physicsRaycaster: return mapper.Map(physicsRaycaster);
                case Button button: return mapper.Map(button);
                case Dropdown dropdown: return mapper.Map(dropdown);
                case GraphicRaycaster graphicRaycaster: return mapper.Map(graphicRaycaster);
                case InputField inputField: return mapper.Map(inputField);
                case Scrollbar scrollbar: return mapper.Map(scrollbar);
                case Slider slider: return mapper.Map(slider);
                case Toggle toggle: return mapper.Map(toggle);
                case GridLayoutGroup gridLayoutGroup: return mapper.Map(gridLayoutGroup);
                case PositionAsUV1 positionAsUV1: return mapper.Map(positionAsUV1);
                case Shadow shadow: return mapper.Map(shadow);
                case SpringJoint2D springJoint2D: return mapper.Map(springJoint2D);
                case DistanceJoint2D distanceJoint2D: return mapper.Map(distanceJoint2D);
                case FrictionJoint2D frictionJoint2D: return mapper.Map(frictionJoint2D);
                case HingeJoint2D hingeJoint2D: return mapper.Map(hingeJoint2D);
                case SliderJoint2D sliderJoint2D: return mapper.Map(sliderJoint2D);
                case FixedJoint2D fixedJoint2D: return mapper.Map(fixedJoint2D);
                case WheelJoint2D wheelJoint2D: return mapper.Map(wheelJoint2D);
                case EventSystem eventSystem: return mapper.Map(eventSystem);
                case BaseInput baseInput: return mapper.Map(baseInput);
                case Mask mask: return mapper.Map(mask);
                case RectMask2D rectMask2D: return mapper.Map(rectMask2D);
                case ScrollRect scrollRect: return mapper.Map(scrollRect);
                case Selectable selectable: return mapper.Map(selectable);
                case ToggleGroup toggleGroup: return mapper.Map(toggleGroup);
                case AspectRatioFitter aspectRatioFitter: return mapper.Map(aspectRatioFitter);
                case CanvasScaler canvasScaler: return mapper.Map(canvasScaler);
                case ContentSizeFitter contentSizeFitter: return mapper.Map(contentSizeFitter);
                case LayoutElement layoutElement: return mapper.Map(layoutElement);
                case AudioListener audioListener: return mapper.Map(audioListener);
                case AudioSource audioSource: return mapper.Map(audioSource);
                case Grid grid: return mapper.Map(grid);
                case CircleCollider2D circleCollider2D: return mapper.Map(circleCollider2D);
                case CapsuleCollider2D capsuleCollider2D: return mapper.Map(capsuleCollider2D);
                case EdgeCollider2D edgeCollider2D: return mapper.Map(edgeCollider2D);
                case BoxCollider2D boxCollider2D: return mapper.Map(boxCollider2D);
                case PolygonCollider2D polygonCollider2D: return mapper.Map(polygonCollider2D);
                case CompositeCollider2D compositeCollider2D: return mapper.Map(compositeCollider2D);
                case AnchoredJoint2D anchoredJoint2D: return mapper.Map(anchoredJoint2D);
                case RelativeJoint2D relativeJoint2D: return mapper.Map(relativeJoint2D);
                case TargetJoint2D targetJoint2D: return mapper.Map(targetJoint2D);
                case AreaEffector2D areaEffector2D: return mapper.Map(areaEffector2D);
                case BuoyancyEffector2D buoyancyEffector2D: return mapper.Map(buoyancyEffector2D);
                case PointEffector2D pointEffector2D: return mapper.Map(pointEffector2D);
                case PlatformEffector2D platformEffector2D: return mapper.Map(platformEffector2D);
                case SurfaceEffector2D surfaceEffector2D: return mapper.Map(surfaceEffector2D);
                case ConstantForce2D constantForce2D: return mapper.Map(constantForce2D);
                case EventTrigger eventTrigger: return mapper.Map(eventTrigger);
                case NavMeshAgent navMeshAgent: return mapper.Map(navMeshAgent);
                case NavMeshObstacle navMeshObstacle: return mapper.Map(navMeshObstacle);
                case OffMeshLink offMeshLink: return mapper.Map(offMeshLink);
                case Animation animation: return mapper.Map(animation);
                case Animator animator: return mapper.Map(animator);
                case AimConstraint aimConstraint: return mapper.Map(aimConstraint);
                case PositionConstraint positionConstraint: return mapper.Map(positionConstraint);
                case RotationConstraint rotationConstraint: return mapper.Map(rotationConstraint);
                case ScaleConstraint scaleConstraint: return mapper.Map(scaleConstraint);
                case LookAtConstraint lookAtConstraint: return mapper.Map(lookAtConstraint);
                case ParentConstraint parentConstraint: return mapper.Map(parentConstraint);
                case AudioBehaviour audioBehaviour: return mapper.Map(audioBehaviour);
                case AudioReverbZone audioReverbZone: return mapper.Map(audioReverbZone);
                case AudioLowPassFilter audioLowPassFilter: return mapper.Map(audioLowPassFilter);
                case AudioHighPassFilter audioHighPassFilter: return mapper.Map(audioHighPassFilter);
                case AudioDistortionFilter audioDistortionFilter: return mapper.Map(audioDistortionFilter);
                case AudioEchoFilter audioEchoFilter: return mapper.Map(audioEchoFilter);
                case AudioChorusFilter audioChorusFilter: return mapper.Map(audioChorusFilter);
                case AudioReverbFilter audioReverbFilter: return mapper.Map(audioReverbFilter);
                case BillboardRenderer billboardRenderer: return mapper.Map(billboardRenderer);
                case Camera camera: return mapper.Map(camera);
                case FlareLayer flareLayer: return mapper.Map(flareLayer);
                case Projector projector: return mapper.Map(projector);
                case TrailRenderer trailRenderer: return mapper.Map(trailRenderer);
                case LineRenderer lineRenderer: return mapper.Map(lineRenderer);
                case LensFlare lensFlare: return mapper.Map(lensFlare);
                case Light light: return mapper.Map(light);
                case Skybox skybox: return mapper.Map(skybox);
                case LightProbeProxyVolume lightProbeProxyVolume: return mapper.Map(lightProbeProxyVolume);
                case SkinnedMeshRenderer skinnedMeshRenderer: return mapper.Map(skinnedMeshRenderer);
                case MeshRenderer meshRenderer: return mapper.Map(meshRenderer);
                case GUIElement gUIElement: return mapper.Map(gUIElement);
                case LightProbeGroup lightProbeGroup: return mapper.Map(lightProbeGroup);
                case MonoBehaviour monoBehaviour: return mapper.Map(monoBehaviour);
                case ReflectionProbe reflectionProbe: return mapper.Map(reflectionProbe);
                case RectTransform rectTransform: return mapper.Map(rectTransform);
                case SpriteRenderer spriteRenderer: return mapper.Map(spriteRenderer);
                case SortingGroup sortingGroup: return mapper.Map(sortingGroup);
                case GridLayout gridLayout: return mapper.Map(gridLayout);
                case CharacterController characterController: return mapper.Map(characterController);
                case MeshCollider meshCollider: return mapper.Map(meshCollider);
                case CapsuleCollider capsuleCollider: return mapper.Map(capsuleCollider);
                case BoxCollider boxCollider: return mapper.Map(boxCollider);
                case SphereCollider sphereCollider: return mapper.Map(sphereCollider);
                case ConstantForce constantForce: return mapper.Map(constantForce);
                case HingeJoint hingeJoint: return mapper.Map(hingeJoint);
                case SpringJoint springJoint: return mapper.Map(springJoint);
                case FixedJoint fixedJoint: return mapper.Map(fixedJoint);
                case CharacterJoint characterJoint: return mapper.Map(characterJoint);
                case ConfigurableJoint configurableJoint: return mapper.Map(configurableJoint);
                case Collider2D collider2D: return mapper.Map(collider2D);
                case Joint2D joint2D: return mapper.Map(joint2D);
                case Effector2D effector2D: return mapper.Map(effector2D);
                case PhysicsUpdateBehaviour2D physicsUpdateBehaviour2D: return mapper.Map(physicsUpdateBehaviour2D);
                case Canvas canvas: return mapper.Map(canvas);
                case CanvasGroup canvasGroup: return mapper.Map(canvasGroup);
                case VideoPlayer videoPlayer: return mapper.Map(videoPlayer);
                case Behaviour behaviour: return mapper.Map(behaviour);
                case Renderer renderer: return mapper.Map(renderer);
                case OcclusionPortal occlusionPortal: return mapper.Map(occlusionPortal);
                case OcclusionArea occlusionArea: return mapper.Map(occlusionArea);
                case MeshFilter meshFilter: return mapper.Map(meshFilter);
                case LODGroup lODGroup: return mapper.Map(lODGroup);
                case Transform transform: return mapper.Map(transform);
                case Rigidbody rigidbody: return mapper.Map(rigidbody);
                case Collider collider: return mapper.Map(collider);
                case Joint joint: return mapper.Map(joint);
                case Rigidbody2D rigidbody2D: return mapper.Map(rigidbody2D);
                case CanvasRenderer canvasRenderer: return mapper.Map(canvasRenderer);
                default: return default;
            }
        }

        public static IComponent ToComponent(this Component component)
        {
            switch (component)
            {
                case StandaloneInputModule standaloneInputModule: return standaloneInputModule.ToComponent();
                case Physics2DRaycaster physics2DRaycaster: return physics2DRaycaster.ToComponent();
                case Image image: return image.ToComponent();
                case RawImage rawImage: return rawImage.ToComponent();
                case Text text: return text.ToComponent();
                case HorizontalLayoutGroup horizontalLayoutGroup: return horizontalLayoutGroup.ToComponent();
                case VerticalLayoutGroup verticalLayoutGroup: return verticalLayoutGroup.ToComponent();
                case Outline outline: return outline.ToComponent();
                case PhysicsRaycaster physicsRaycaster: return physicsRaycaster.ToComponent();
                case Button button: return button.ToComponent();
                case Dropdown dropdown: return dropdown.ToComponent();
                case GraphicRaycaster graphicRaycaster: return graphicRaycaster.ToComponent();
                case InputField inputField: return inputField.ToComponent();
                case Scrollbar scrollbar: return scrollbar.ToComponent();
                case Slider slider: return slider.ToComponent();
                case Toggle toggle: return toggle.ToComponent();
                case GridLayoutGroup gridLayoutGroup: return gridLayoutGroup.ToComponent();
                case PositionAsUV1 positionAsUV1: return positionAsUV1.ToComponent();
                case Shadow shadow: return shadow.ToComponent();
                case SpringJoint2D springJoint2D: return springJoint2D.ToComponent();
                case DistanceJoint2D distanceJoint2D: return distanceJoint2D.ToComponent();
                case FrictionJoint2D frictionJoint2D: return frictionJoint2D.ToComponent();
                case HingeJoint2D hingeJoint2D: return hingeJoint2D.ToComponent();
                case SliderJoint2D sliderJoint2D: return sliderJoint2D.ToComponent();
                case FixedJoint2D fixedJoint2D: return fixedJoint2D.ToComponent();
                case WheelJoint2D wheelJoint2D: return wheelJoint2D.ToComponent();
                case EventSystem eventSystem: return eventSystem.ToComponent();
                case BaseInput baseInput: return baseInput.ToComponent();
                case Mask mask: return mask.ToComponent();
                case RectMask2D rectMask2D: return rectMask2D.ToComponent();
                case ScrollRect scrollRect: return scrollRect.ToComponent();
                case Selectable selectable: return selectable.ToComponent();
                case ToggleGroup toggleGroup: return toggleGroup.ToComponent();
                case AspectRatioFitter aspectRatioFitter: return aspectRatioFitter.ToComponent();
                case CanvasScaler canvasScaler: return canvasScaler.ToComponent();
                case ContentSizeFitter contentSizeFitter: return contentSizeFitter.ToComponent();
                case LayoutElement layoutElement: return layoutElement.ToComponent();
                case AudioListener audioListener: return audioListener.ToComponent();
                case AudioSource audioSource: return audioSource.ToComponent();
                case Grid grid: return grid.ToComponent();
                case CircleCollider2D circleCollider2D: return circleCollider2D.ToComponent();
                case CapsuleCollider2D capsuleCollider2D: return capsuleCollider2D.ToComponent();
                case EdgeCollider2D edgeCollider2D: return edgeCollider2D.ToComponent();
                case BoxCollider2D boxCollider2D: return boxCollider2D.ToComponent();
                case PolygonCollider2D polygonCollider2D: return polygonCollider2D.ToComponent();
                case CompositeCollider2D compositeCollider2D: return compositeCollider2D.ToComponent();
                case AnchoredJoint2D anchoredJoint2D: return anchoredJoint2D.ToComponent();
                case RelativeJoint2D relativeJoint2D: return relativeJoint2D.ToComponent();
                case TargetJoint2D targetJoint2D: return targetJoint2D.ToComponent();
                case AreaEffector2D areaEffector2D: return areaEffector2D.ToComponent();
                case BuoyancyEffector2D buoyancyEffector2D: return buoyancyEffector2D.ToComponent();
                case PointEffector2D pointEffector2D: return pointEffector2D.ToComponent();
                case PlatformEffector2D platformEffector2D: return platformEffector2D.ToComponent();
                case SurfaceEffector2D surfaceEffector2D: return surfaceEffector2D.ToComponent();
                case ConstantForce2D constantForce2D: return constantForce2D.ToComponent();
                case EventTrigger eventTrigger: return eventTrigger.ToComponent();
                case NavMeshAgent navMeshAgent: return navMeshAgent.ToComponent();
                case NavMeshObstacle navMeshObstacle: return navMeshObstacle.ToComponent();
                case OffMeshLink offMeshLink: return offMeshLink.ToComponent();
                case Animation animation: return animation.ToComponent();
                case Animator animator: return animator.ToComponent();
                case AimConstraint aimConstraint: return aimConstraint.ToComponent();
                case PositionConstraint positionConstraint: return positionConstraint.ToComponent();
                case RotationConstraint rotationConstraint: return rotationConstraint.ToComponent();
                case ScaleConstraint scaleConstraint: return scaleConstraint.ToComponent();
                case LookAtConstraint lookAtConstraint: return lookAtConstraint.ToComponent();
                case ParentConstraint parentConstraint: return parentConstraint.ToComponent();
                case AudioBehaviour audioBehaviour: return audioBehaviour.ToComponent();
                case AudioReverbZone audioReverbZone: return audioReverbZone.ToComponent();
                case AudioLowPassFilter audioLowPassFilter: return audioLowPassFilter.ToComponent();
                case AudioHighPassFilter audioHighPassFilter: return audioHighPassFilter.ToComponent();
                case AudioDistortionFilter audioDistortionFilter: return audioDistortionFilter.ToComponent();
                case AudioEchoFilter audioEchoFilter: return audioEchoFilter.ToComponent();
                case AudioChorusFilter audioChorusFilter: return audioChorusFilter.ToComponent();
                case AudioReverbFilter audioReverbFilter: return audioReverbFilter.ToComponent();
                case BillboardRenderer billboardRenderer: return billboardRenderer.ToComponent();
                case Camera camera: return camera.ToComponent();
                case FlareLayer flareLayer: return flareLayer.ToComponent();
                case Projector projector: return projector.ToComponent();
                case TrailRenderer trailRenderer: return trailRenderer.ToComponent();
                case LineRenderer lineRenderer: return lineRenderer.ToComponent();
                case LensFlare lensFlare: return lensFlare.ToComponent();
                case Light light: return light.ToComponent();
                case Skybox skybox: return skybox.ToComponent();
                case LightProbeProxyVolume lightProbeProxyVolume: return lightProbeProxyVolume.ToComponent();
                case SkinnedMeshRenderer skinnedMeshRenderer: return skinnedMeshRenderer.ToComponent();
                case MeshRenderer meshRenderer: return meshRenderer.ToComponent();
                case GUIElement gUIElement: return gUIElement.ToComponent();
                case LightProbeGroup lightProbeGroup: return lightProbeGroup.ToComponent();
                case MonoBehaviour monoBehaviour: return monoBehaviour.ToComponent();
                case ReflectionProbe reflectionProbe: return reflectionProbe.ToComponent();
                case RectTransform rectTransform: return rectTransform.ToComponent();
                case SpriteRenderer spriteRenderer: return spriteRenderer.ToComponent();
                case SortingGroup sortingGroup: return sortingGroup.ToComponent();
                case GridLayout gridLayout: return gridLayout.ToComponent();
                case CharacterController characterController: return characterController.ToComponent();
                case MeshCollider meshCollider: return meshCollider.ToComponent();
                case CapsuleCollider capsuleCollider: return capsuleCollider.ToComponent();
                case BoxCollider boxCollider: return boxCollider.ToComponent();
                case SphereCollider sphereCollider: return sphereCollider.ToComponent();
                case ConstantForce constantForce: return constantForce.ToComponent();
                case HingeJoint hingeJoint: return hingeJoint.ToComponent();
                case SpringJoint springJoint: return springJoint.ToComponent();
                case FixedJoint fixedJoint: return fixedJoint.ToComponent();
                case CharacterJoint characterJoint: return characterJoint.ToComponent();
                case ConfigurableJoint configurableJoint: return configurableJoint.ToComponent();
                case Collider2D collider2D: return collider2D.ToComponent();
                case Joint2D joint2D: return joint2D.ToComponent();
                case Effector2D effector2D: return effector2D.ToComponent();
                case PhysicsUpdateBehaviour2D physicsUpdateBehaviour2D: return physicsUpdateBehaviour2D.ToComponent();
                case Canvas canvas: return canvas.ToComponent();
                case CanvasGroup canvasGroup: return canvasGroup.ToComponent();
                case VideoPlayer videoPlayer: return videoPlayer.ToComponent();
                case Behaviour behaviour: return behaviour.ToComponent();
                case Renderer renderer: return renderer.ToComponent();
                case OcclusionPortal occlusionPortal: return occlusionPortal.ToComponent();
                case OcclusionArea occlusionArea: return occlusionArea.ToComponent();
                case MeshFilter meshFilter: return meshFilter.ToComponent();
                case LODGroup lODGroup: return lODGroup.ToComponent();
                case Transform transform: return transform.ToComponent();
                case Rigidbody rigidbody: return rigidbody.ToComponent();
                case Collider collider: return collider.ToComponent();
                case Joint joint: return joint.ToComponent();
                case Rigidbody2D rigidbody2D: return rigidbody2D.ToComponent();
                case CanvasRenderer canvasRenderer: return canvasRenderer.ToComponent();
                default: return null;
            }
        }

        public static Unity<T> ToComponent<T>(this T component) where T : Component => new Unity<T> { Value = component };
    }
}
