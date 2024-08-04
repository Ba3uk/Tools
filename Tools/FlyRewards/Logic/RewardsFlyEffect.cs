using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using FlyRewards;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace common.FlyRewards.Logic
{
    internal sealed class RewardsFlyEffect : MonoBehaviour, IRewardsFlyEffect
    {
        [SerializeField] private RewardsFlyBehaviour _example;
        [SerializeField] private TextMeshProUGUI _countLabel;
        public Action FirstComplete;
        public bool IsShowCount;
        private List<RewardsFlyBehaviour> _items = new();
        private Vector2[,] _coords;
        private RewardsFlyEffectPreset _preset;

        public void Prepare(Sprite sprite, int count, RewardsFlyEffectPreset preset)
        {
            if (count < 1)
            {
                Debug.LogError($"ResourceFlyEffect count was {count}, it can't be less than 1");
                count = 1;
            }

            _preset = preset;
            _example.gameObject.SetActive(false);

            _countLabel.gameObject.SetActive(IsShowCount);
            if (IsShowCount)
            {
                _countLabel.text = count.ToString();

                _countLabel.gameObject.SetActive(count > 1);
                count = 1;
            }

            for (int i = _items.Count; i < count; i++)
            {
                var item = Instantiate(_example, _example.transform.parent, true);
                _items.Add(item);
            }

            for (int i = 0; i < _items.Count; i++)
            {
                _items[i].gameObject.SetActive(i < count);
            }

            foreach (var item in _items)
            {
                item.transform.localPosition = Vector3.zero;
                item.Sprite = sprite;
                item.Width = (int) _preset.Size.x;
                item.Height = (int) _preset.Size.y;
            }
        }

        public Tween Fly(Vector3 endPos, Vector2? scale = null)
        {
            if (scale == null)
            {
                scale = Vector2.one;
            }

            endPos.z = -20;
            var sequence = DOTween.Sequence();
            var scaleSequence = DOTween.Sequence();

            _coords = new Vector2[5, _items.Count];
            var lanes = _preset.Lanes != 0 ? _preset.Lanes : _items.Count(a => a.gameObject.activeSelf);
            var laneWidth = _preset.MaxRadius * 2 / lanes;
            for (int i = 0; i < _items.Count; i++)
            {
                var seq = DOTween.Sequence();
                var scaleSeq = DOTween.Sequence();
                var item = _items[i];
                var even = i % 2 == 0;
                //var x = Random.Range(even ? -_preset.MaxRadius : 0, even ? 0 : _preset.MaxRadius);
                var x = Random.Range(-_preset.MaxRadius + laneWidth * (i % lanes),
                    -_preset.MaxRadius + laneWidth * (i % lanes + 1));
                var yMin = Mathf.Sqrt(Mathf.Pow(_preset.MinRadius, 2) - Mathf.Pow(x, 2));
                yMin = float.IsNaN(yMin) || _preset.IgnoreDoubleTraceFly ? 0 : yMin;
                var yMax = Mathf.Sqrt(Mathf.Pow(_preset.MaxRadius, 2) - Mathf.Pow(x, 2));
                var y = Random.Range(yMin, yMax);
                _coords[0, i] = new Vector2(x * _preset.PeakMult.x, y * _preset.PeakMult.y);
                _coords[1, i] = new Vector2(x, y);
                _coords[2, i] = new Vector2(x, y + 5f);
                _coords[3, i] = new Vector2(x, y);
                _coords[4, i] = endPos;

                //item.transform.Rotate(new Vector3(_preset.XRotationCurve.Evaluate(0), _preset.YRotationCurve.Evaluate(0), _preset.ZRotationCurve.Evaluate(0)) * 360);
                item.transform.rotation = new Quaternion(0f, 0f, 0f, 0f);

                var tween1 =
                    item.transform.GetDoubleTween(_coords[0, i], Ease.Linear, _preset.Step1Ease, _preset.Step1);
                var tween2 =
                    item.transform.GetDoubleTween(_coords[1, i], Ease.Linear, _preset.Step2Ease, _preset.Step2);
                var tween3 =
                    item.transform.GetDoubleTween(_coords[2, i], Ease.Linear, Ease.Linear, _preset.BounceHalftime);
                var tween4 =
                    item.transform.GetDoubleTween(_coords[3, i], Ease.Linear, Ease.Linear, _preset.BounceHalftime);

                var scaleDuration = _preset.Step1 + _preset.Step2 + _preset.BounceHalftime + _preset.BounceHalftime +
                                    _preset.FlyTime;
                scaleSeq.Append(item.transform.DOScale(scale.Value, scaleDuration).SetEase(Ease.InQuint));


                if (_preset.EnableRotation)
                {
                    var rotTween = item.transform.GetTweenCurveRotation(
                        _preset.XRotationCurve,
                        _preset.YRotationCurve,
                        _preset.ZRotationCurve,
                        _preset.Step1 + _preset.Step2,
                        _preset.NegateRotationIfXIsLowerThanZero && x < 0
                    );
                    sequence.Insert(0, rotTween);
                }

                var tween5 = (_preset.IgnoreDoubleTraceFly)
                    ? item.transform.GetDoubleTween(_coords[4, i], _preset.FlyEase, _preset.FlyEase, _preset.FlyTime,
                        false)
                    : item.transform.GetDoubleTween(_coords[4, i], even ? Ease.Linear : _preset.FlyEase,
                        even ? _preset.FlyEase : Ease.Linear, _preset.FlyTime, false);

                tween1.SetDelay(i * _preset.Step1ItemDelay);
                tween5.SetDelay(_preset.Delay);
                if (i == 0)
                    tween5.OnComplete(() => FirstComplete?.Invoke());

                seq.Append(tween1);
                seq.Append(tween2);
                seq.Append(tween3);
                seq.Append(tween4);
                seq.Append(tween5);

                scaleSequence.Insert(0, scaleSeq);
                sequence.Insert(0, seq);
            }

            sequence.OnKill(Complete);
            sequence.Play();
            scaleSequence.Play();
            return sequence;
        }

        private void Complete()
        {
            Delete();
        }

        private void Delete()
        {
            Destroy(gameObject);
        }

        public void SetActive(bool value)
        {
            gameObject.SetActive(value);
        }
    }
}