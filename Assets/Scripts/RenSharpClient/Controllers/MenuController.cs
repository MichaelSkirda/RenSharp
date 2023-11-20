using RenSharp;
using RenSharp.Core;
using RenSharp.RenSharpClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RenSharpClient.Controllers
{
	public class MenuController : MonoBehaviour
	{
		[SerializeField]
		private Transform Parent;
		[SerializeField]
		private GameObject ButtonPrefab;

		public void Clear()
		{
			foreach (Transform child in Parent.transform)
			{
				Destroy(child.gameObject);
			}
		}

		public void Show(IEnumerable<MenuButton> buttonsEnum, RenSharpCore core)
		{
			Configuration config = core.Configuration;
			List<MenuButton> menuButtons = buttonsEnum.ToList();
			float gap = config.GetValueOrDefault<float>("gui_btn_gap");
			float firstGap = config.GetValueOrDefault<float>("gui_btn_first_gap");

			for (int i = 0; i < menuButtons.Count; i++)
			{
				var menuButton = menuButtons[i];
				GameObject buttonObj = Instantiate(ButtonPrefab, Parent);

				TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
				Button button = buttonObj.GetComponent<Button>();

				button.onClick.AddListener(() =>
				{
					Clear();
					core.Goto(menuButton.Label);
					core.ReadNext(true);
					core.Resume();
				});

				if (text == null)
					throw new ArgumentException("Не найден компонент текст у кнопки.");

				RectTransform rect = buttonObj.GetComponent<RectTransform>();

				float height = rect.rect.height;

				text.text = menuButton.Text;

				float y = i * (gap + height) + height / 2 + firstGap;
				rect.anchoredPosition = new Vector2(x: 0, -y);
			}
		}
	}
}