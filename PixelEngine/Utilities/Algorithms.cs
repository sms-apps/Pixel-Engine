using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixelEngine.Utilities {
	/// <summary> PixelEngine Class to hold some helper algorithms </summary>
	public static class Algorithms {
		/// <summary> Concat some number of arrays together </summary>
		/// <typeparam name="T"> Generic type parameter </typeparam>
		/// <param name="items"> Arrays to combine, comma separated </param>
		/// <returns> One array, with content of all given arrays </returns>
		public static T[] Concat<T>(params T[][] items) {
			T[] result = new T[items.Sum(t => t.Length)];
			int counter = 0;

			foreach (T[] item in items) {
				foreach (T t in item) { result[counter++] = t; }
			}
			return result;
		}
		/// <summary> Concat some number of Lists together </summary>
		/// <typeparam name="T"> Generic type parameter </typeparam>
		/// <param name="items"> Lists to combine, comma separated </param>
		/// <returns> One List, with content of all given Lists </returns>
		public static List<T> Concat<T>(params List<T>[] items) {
			List<T> result = new List<T>(items.Sum(t => t.Count));
			int counter = 0;

			foreach (List<T> item in items) {
				foreach (T t in item) { result[counter++] = t; }
			}
			return result;
		}

		/// <summary> Sort a given array </summary> <typeparam name="T"> Generic type parameter </typeparam> <param name="items"> Array to sort </param>
		public static void Sort<T>(this T[] items) { Sort(items, Comparer<T>.Default.Compare); }
		/// <summary> Sort a given list </summary> <typeparam name="T"> Generic type parameter </typeparam> <param name="items"> List to sort </param>
		public static void Sort<T>(this List<T> items) { Sort(items, Comparer<T>.Default.Compare); }
		/// <summary> Sort a given array with a given comparison function </summary> <typeparam name="T"> Generic type parameter </typeparam> 
		/// <param name="items"> Array to sort </param><param name="comparision">Custom compare function to use</param>
		public static void Sort<T>(this T[] items, Comparison<T> comparision) {
			/// Helper function for quicksort partitioning 
			int Partition(T[] arr, int left, int right) {
				T pivot = arr[(left+right)/2]; // Less likely to pick a bad pivot on sorted data.

				while (true) {
					while (comparision(arr[left], pivot) < 0) { left++; }
					while (comparision(arr[right], pivot) > 0) { right--; }

					if (left < right) {
						if (comparision(arr[left], arr[right]) == 0) { return right; }

						T temp = arr[left];
						arr[left] = arr[right];
						arr[right] = temp;
					} else {
						return right;
					}
				}
			}
			/// Recursive helper 
			void QuickSort(T[] arr, int left, int right) {
				if (left < right) {
					int pivot = Partition(arr, left, right);
					if (pivot > 1) { QuickSort(arr, left, pivot - 1); }
					if (pivot + 1 < right) { QuickSort(arr, pivot + 1, right); }
				}
			}
			// Kick off recursive quicksort
			QuickSort(items, 0, items.Length - 1);
		}

		/// <summary> Sort a given List with a given comparison function </summary> <typeparam name="T"> Generic type parameter </typeparam> 
		/// <param name="items"> List to sort </param><param name="comparision">Custom compare function to use</param>
		public static void Sort<T>(this List<T> items, Comparison<T> comparision) {
			/// Helper function for quicksort partitioning 
			int Partition(List<T> arr, int left, int right) {
				T pivot = arr[(left+right)/2]; // Less likely to pick a bad pivot on sorted data

				while (true) {
					while (comparision(arr[left], pivot) < 0) { left++; }
					while (comparision(arr[right], pivot) > 0) { right--; }

					if (left < right) {
						if (comparision(arr[left], arr[right]) == 0) { return right; }

						T temp = arr[left];
						arr[left] = arr[right];
						arr[right] = temp;
					} else {
						return right;
					}
				}
			}
			/// Recursive helper 
			void QuickSort(List<T> arr, int left, int right) {
				if (left < right) {
					int pivot = Partition(arr, left, right);
					if (pivot > 1) { QuickSort(arr, left, pivot - 1); }
					if (pivot + 1 < right) { QuickSort(arr, pivot + 1, right); }
				}
			}
			// Kick off recursive quicksort
			QuickSort(items, 0, items.Count - 1);
		}

		/// <summary> Randomize an array by performing <see cref="Array.Length"/> random swaps. </summary>
		/// <typeparam name="T"> Generic type parameter </typeparam>
		/// <param name="items"> Array to randomize </param>
		public static void Randomize<T>(this T[] items) {
			for (int i = 0; i < items.Length; i++) {
				int r = Randoms.RandomInt(i, items.Length);

				T temp = items[i];
				items[i] = items[r];
				items[r] = temp;
			}
		}
		/// <summary> Randomize a list by performing <see cref="List{T}.Count"/> random swaps. </summary>
		/// <typeparam name="T"> Generic type parameter </typeparam>
		/// <param name="items"> List to randomize </param>
		public static void Randomize<T>(this List<T> items) {
			for (int i = 0; i < items.Count; i++) {
				int r = Randoms.RandomInt(i, items.Count);

				T temp = items[i];
				items[i] = items[r];
				items[r] = temp;
			}
		}

		/// <summary> Search a collection for a given item. </summary> <typeparam name="T"> Generic type parameter </typeparam>
		/// <param name="items"> Collection to search </param> <param name="item"> Item to find </param>
		/// <returns> First matching item if found, or default value if not found </returns>
		public static T Search<T>(this IEnumerable<T> items, T item) { return  Search(items, (t, i) => t.Equals(item)); }
		/// <summary> Search a collection for an item matching a condition. </summary> <typeparam name="T"> Generic type parameter </typeparam>
		/// <param name="items"> Collection to search </param> <param name="condition"> Condition to match </param>
		/// <returns> first item matching condition if found, or default value if not found </returns>
		public static T Search<T>(this IEnumerable<T> items, Func<T, bool> condition) { return Search(items, (t, i) => condition(t)); }
		/// <summary> Search a collection for an item matching a condition. </summary> <typeparam name="T"> Generic type parameter </typeparam>
		/// <param name="items"> Collection to search </param> <param name="condition"> Condition to match </param>
		/// <returns> first item matching condition if found, or default value if not found </returns>
		public static T Search<T>(this IEnumerable<T> items, Func<T, int, bool> condition) {
			int index = 0;

			foreach (T item in items) {
				if (condition(item, index++)) {
					return item;
				}
			}

			return default;
		}
	}
}
