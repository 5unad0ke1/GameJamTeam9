using UnityEngine;
using System;

/// <summary>
/// 選択肢の状態管理と選択ロジックを担当するクラス。
/// </summary>
public class OptionSelector
{
    private int _selectedIndex;
    private readonly int _optionCount;

    public int SelectedIndex => _selectedIndex;
    public Action<int> OnSelectionChanged;

    public OptionSelector(int optionCount, int initialIndex = 0)
    {
        _optionCount = optionCount;
        _selectedIndex = Mathf.Clamp(initialIndex, 0, _optionCount - 1);
    }

    /// <summary>
    /// 次の選択肢を選択します。
    /// </summary>
    /// <returns>選択が変更された場合はtrue</returns>
    public bool SelectNext()
    {
        if (_selectedIndex >= _optionCount - 1)
        {
            return false;
        }

        _selectedIndex++;
        OnSelectionChanged?.Invoke(_selectedIndex);
        return true;
    }

    /// <summary>
    /// 前の選択肢を選択します。
    /// </summary>
    /// <returns>選択が変更された場合はtrue</returns>
    public bool SelectPrevious()
    {
        if (_selectedIndex <= 0)
        {
            return false;
        }

        _selectedIndex--;
        OnSelectionChanged?.Invoke(_selectedIndex);
        return true;
    }

    /// <summary>
    /// 指定したインデックスを選択します。
    /// </summary>
    public void SelectIndex(int index)
    {
        int newIndex = Mathf.Clamp(index, 0, _optionCount - 1);
        if (_selectedIndex != newIndex)
        {
            _selectedIndex = newIndex;
            OnSelectionChanged?.Invoke(_selectedIndex);
        }
    }
}
