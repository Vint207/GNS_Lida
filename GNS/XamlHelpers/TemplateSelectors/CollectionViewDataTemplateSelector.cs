using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNS.Services.TemplateSelectors
{
	public class CollectionViewDataTemplateSelector : DataTemplateSelector
	{
		public DataTemplate EvenTemplate { get; set; } // Шаблон для четных элементов
		public DataTemplate OddTemplate { get; set; }  // Шаблон для нечетных элементов

		protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
		{
			// Получаем элемент визуального дерева
			if (container is Element element)
			{
				var parent = element;

				// Ищем родительский CollectionView
				while (parent is not CollectionView)
				{
					parent = parent.Parent;
				}

				if (parent is CollectionView collectionView && collectionView.ItemsSource is IList items)
				{
					var index = items.IndexOf(item);

					// Проверяем, является ли индекс четным или нечетным
					if (index % 2 == 0)
						return EvenTemplate;
					else
						return OddTemplate;
				}
			}

			// По умолчанию возвращаем шаблон для четных элементов
			return EvenTemplate;
		}
	}
}
