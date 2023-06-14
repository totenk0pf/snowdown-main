using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Client.UI {
    public class WeaponPanel : MonoBehaviour {
        public TextMeshProUGUI indexText;
        public Image weaponImage;

        [SerializeField] private DOTweenAnimation[] anims;

        public void ToggleState(bool state) {
            if (state) OnSelect();
            else OnDeselect();
        }

        private void OnSelect() {
            foreach (DOTweenAnimation anim in anims) {
                anim.DOPlay();
            }
        }

        private void OnDeselect() {
            foreach (DOTweenAnimation anim in anims) {
                anim.DOPlayBackwards();
            } 
        }
    }
}