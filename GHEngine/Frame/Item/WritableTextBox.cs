using GHEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GHEngine.Frame.Item;

public class WritableTextBox : TextBox, ITimeUpdatable
{
    // Static fields.
    public static readonly TimeSpan CURSOR_BLINK_DELAY_DEFAULT = TimeSpan.FromSeconds(0.5d);
    public static readonly TimeSpan NAVIGATION_DELAY_INITIAL_DEFAULT = TimeSpan.FromSeconds(0.5d);
    public static readonly TimeSpan NAVIGATION_DELAY_REPEAT_DEFAULT = TimeSpan.FromSeconds(0.05d);
    public const float CURSOR_DEFAULT_RELATIVE_THICKNESS = 0.05f;


    // Fields.
    public TimeSpan CursorBlinkDelay { get; set; } = CURSOR_BLINK_DELAY_DEFAULT;

    public bool IsTypingEnabled
    {
        get => _isTypingEnabled;
        set
        {
            if (value == _isTypingEnabled)
            {
                return;
            }

            _isTypingEnabled = value;
            if (value)
            {
                _userInput.TextInput += OnKeyInputEvent;
            }
            else
            {
                _userInput.TextInput -= OnKeyInputEvent;
            }
        }
    }

    public int CursorIndexMin
    {
        get => _cursor.IndexMin;
        set => _cursor.IndexMin = value;
    }

    public int CursorIndexMax
    {
        get => _cursor.IndexMax;
        set => _cursor.IndexMax = value;
    }

    public bool IsPastingAllowed { get; set; } = true;
    public bool IsCopyingAllowed { get; set; } = true;
    public bool IsCuttingAllowed { get; set; } = true;
    public bool IsKeyNavigationAllowed { get; set; } = true;
    public bool IsMouseNavigationAllowed { get; set; } = true;
    public bool IsSelectingAllowed { get; set; } = true;

    public bool IsFocused
    {
        get => _isFocused;
        set => _isFocused = value;
    }

    public float CursorRelativeThickness
    {
        get => _cursor.CursorRelativeThickness;
        set => _cursor.CursorRelativeThickness = value;
    }

    public bool IsSelectionMade => _cursor.IsSelectionMode;
    public bool IsTextInserted { get; set; } = false;

    public TimeSpan NavigationDelayInitial { get; set; } = NAVIGATION_DELAY_INITIAL_DEFAULT;
    public TimeSpan NavigationDelayRepeat { get; set; } = NAVIGATION_DELAY_REPEAT_DEFAULT;


    // Private fields.
    private bool _isTypingEnabled = false;
    private readonly IUserInput _userInput;
    private bool _isFocused = false;
    private TextCursor _cursor = new();

    private double _navigationDelaySeconds = 0d;
    private NavigationDelayType _navigationDelayType = NavigationDelayType.NoDelay;


    // Constructors.
    public WritableTextBox(IUserInput userInput)
    {
        _userInput = userInput ?? throw new ArgumentNullException(nameof(userInput));
    }


    // Methods.


    // Private methods.
    /* Cursor. */
    private void UpdateBlinkerRenderCache(float aspectRatio)
    {
        int TextIndex = 0;
        foreach (DrawLine Line in DrawLines)
        {
            for (int i = 0; i < Line.Components.Count; i++)
            {
                TextComponent Component = Line.Components[i];
                if ((_cursor.IndexMax >= Component.Text.Length + TextIndex) && (i < Line.Components.Count - 1))
                {
                    TextIndex += Component.Text.Length;
                    continue;
                }

                Vector2 DrawPosition = GetDrawPosition(DrawLines, Line, Component);
                Vector2 ComponentDrawSize = Component.CalculateDrawSize(Component.Text.Substring(0, _cursor.IndexMax - TextIndex));

                Vector2 MaxPoint = GHMath.GetWindowAdjustedVector(new(ComponentDrawSize.X, 0f), aspectRatio);
                float X = DrawPosition.X + GHMath.GetWindowAdjustedVector(new (ComponentDrawSize.X, 0f), aspectRatio).X;

                _cursor.BlinkerRelativeDrawPositionMin = new Vector2(X, DrawPosition.Y);
                _cursor.BlinkerRelativeDrawPositionMax = new Vector2(X, DrawPosition.Y + Component.FontSize);
                UpdateCursorTargets();
                return;
            }
        }
    }

    private Color GetCursorColor(Color textColor)
    {
        int AverageColor = (textColor.R + textColor.G + textColor.B) / 3;
        if (AverageColor >= byte.MaxValue / 2)
        {
            return Color.Black;
        }
        return Color.White;
    }

    private void SetSingleSelection(int index)
    {
        _cursor.IndexMin = Math.Clamp(index, 0, Length);
        _cursor.IndexMax = _cursor.IndexMin;

        UpdateCursorTargets();
        InvalidateBlinkerCache();
        ResetBlinkTimer();
    }

    private void MoveSingleCursor(int steps)
    {
        SetSingleSelection(_cursor.IndexMax + steps);
    }

    private void UpdateCursorTargets()
    {
        _cursor.IndexTargetMin = GetTarget(_cursor.IndexMin);
        _cursor.IndexTargetMax = GetTarget(_cursor.IndexMax);
    }

    private CursorTarget GetTarget(int targetIndex)
    {
        int CurrentIndex = 0;
        foreach (TextComponent Component in this)
        {
            if (targetIndex < Component.Text.Length + CurrentIndex)
            {
                return new(Component, targetIndex - CurrentIndex);
            }
            CurrentIndex += Component.Text.Length;
        }

        return new(this.First(), 0);
    }

    private void InvalidateBlinkerCache()
    {
        _cursor.BlinkerRelativeDrawPositionMin = null;
        _cursor.BlinkerRelativeDrawPositionMax = null;
    }

    private void ResetBlinkTimer()
    {
        _cursor.BlinkerTimer = 0d;
    }

    private void RenderBlinker(IRenderer renderer, IProgramTime time)
    {
        if ((_cursor.BlinkerRelativeDrawPositionMin == null) || (_cursor.BlinkerRelativeDrawPositionMax == null)
            || (_cursor.IndexTargetMin == null) || (_cursor.IndexTargetMax == null))
        {
            UpdateBlinkerRenderCache(renderer.AspectRatio);
        }

        TextComponent TargetedComponent = _cursor.IndexTargetMax!.Component;

        Vector2 MinPos = Vector2.Rotate(_cursor.BlinkerRelativeDrawPositionMin!.Value, Rotation);
        Vector2 MaxPos = Vector2.Rotate(_cursor.BlinkerRelativeDrawPositionMax!.Value, Rotation);

        renderer.DrawLine(
            GetCursorColor(TargetedComponent.Mask),
            MinPos,
            MaxPos,
            TargetedComponent.FontSize * CursorRelativeThickness,
            Shader,
            TargetedComponent.CustomSamplerState);
    }


    /* Typing. */
    private void OnKeyInputEvent(object? sender, TextInputEventArgs args)
    {
        if ((ComponentCount == 0) || !_isTypingEnabled || !_isFocused)
        {
            return;
        }

        ExecuteKeyPress(args.Character, args.Key);
    }

    private void RemoveSelection(int indexMin, int indexMax)
    {

    }

    /* Navigation. */
    private void NavigateToEnd(bool isFullEnd)
    {
        if (isFullEnd)
        {
            SetSingleSelection(Length);
            return;
        }

        for (int i = _cursor.IndexMax; i < Length; i++)
        {
            if (Text[i] != '\n')
            {
                continue;
            }
            SetSingleSelection(i);
            return;
        }
        SetSingleSelection(Length);
    }

    private void NavigateToStart(bool isFullStart)
    {
        if (isFullStart)
        {
            SetSingleSelection(0);
            return;
        }

        for (int i = _cursor.IndexMax; i >= 0; i--)
        {
            if ((i >= Length) || (Text[i] != '\n'))
            {
                continue;
            }

            SetSingleSelection(i + 1);
            return;
        }
        SetSingleSelection(0);
    }

    private bool NavigateKeyboardInstant()
    {
        if (_userInput.WereKeysJustPressed(Keys.End))
        {
            NavigateToEnd(_userInput.AreKeysDown(Keys.LeftControl));
            return true;
        }
        if (_userInput.WereKeysJustPressed(Keys.Home))
        {
            NavigateToStart(_userInput.AreKeysDown(Keys.LeftControl));
            return true;
        }
        return false;
    }

    private bool NavigateKeyboardTimed(IProgramTime time)
    {
        Action? TargetAction = null;

        if (_userInput.AreKeysDown(Keys.Right))
        {
            TargetAction = () => MoveSingleCursor(1);
        }
        if (_userInput.AreKeysDown(Keys.Left))
        {
            TargetAction = () => MoveSingleCursor(-1);
        }

        if (TargetAction != null)
        {
            if (_navigationDelaySeconds <= 0d)
            {
                TargetAction.Invoke();
            }
            return true;
        }

        return false;
    }

    private void NavigateKeyboard(IProgramTime time)
    {
        if (NavigateKeyboardInstant())
        {
            return;
        }

        
        _navigationDelaySeconds = Math.Max(_navigationDelaySeconds - time.PassedTime.TotalSeconds, 0d);
        bool WasNavigationAttempted = WasNavigationAttempted = NavigateKeyboardTimed(time);

        if (WasNavigationAttempted)
        {
            if (_navigationDelayType == NavigationDelayType.NoDelay)
            {
                _navigationDelaySeconds = NavigationDelayInitial.TotalSeconds;
                _navigationDelayType = NavigationDelayType.InitialDelay;
            }
            else if (_navigationDelaySeconds <= 0d)
            {
                _navigationDelayType = NavigationDelayType.RepeatedDelay;
                _navigationDelaySeconds = NavigationDelayRepeat.TotalSeconds;
            }
        }
        else
        {
            _navigationDelayType = NavigationDelayType.NoDelay;
            _navigationDelaySeconds = 0d;
        }
    }

    private void TypeCharacter(char character)
    {
        if (_cursor.IndexTargetMax == null)
        {
            UpdateCursorTargets();
        }
        CursorTarget Target = _cursor.IndexTargetMin!;

        string OriginalText = Target.Component.Text;
        int LeftIndex = Target.TextIndex;
        int RightIndex = Math.Min(Target.TextIndex + (IsTextInserted ? 1 : 0), OriginalText.Length);
        
        string LeftPart = OriginalText.Substring(0, LeftIndex);
        string RightPart = OriginalText.Substring(RightIndex, OriginalText.Length - RightIndex);
        string CombinedText = $"{LeftPart}{character}{RightPart}";

        Target.Component.Text = CombinedText;

        MoveSingleCursor(1);
        ResetBlinkTimer();
    }

    private void DeleteCharacter()
    {

    }

    private void ExecuteKeyPress(char character, Keys key)
    {
        if (IsSelectionMade)
        {
            RemoveSelection(_cursor.IndexMin, _cursor.IndexMax);
        }

        if ((key == Keys.Back) || (key == Keys.Delete))
        {
            DeleteCharacter();
        }
        else
        {
            TypeCharacter(character);
        }
    }


    // Inherited methods.
    public void Update(IProgramTime time)
    {
        _cursor.BlinkerTimer = (_cursor.BlinkerTimer + time.PassedTime.TotalSeconds) % (CursorBlinkDelay.TotalSeconds * 2d);

        if (IsFocused && IsKeyNavigationAllowed)
        {
            NavigateKeyboard(time);
        }
    }

    public override void Render(IRenderer renderer, IProgramTime time)
    {
        base.Render(renderer, time);

        if (IsFocused && IsTypingEnabled && (_cursor.BlinkerTimer <= CursorBlinkDelay.TotalSeconds))
        {
            RenderBlinker(renderer, time);
        }
    }


    // Types.
    private record class CursorTarget(TextComponent Component, int TextIndex);

    private class TextCursor
    {
        // Fields.
        public int IndexMin { get; set; }

        public int IndexMax { get; set; }
        public CursorTarget? IndexTargetMin { get; set; }
        public CursorTarget? IndexTargetMax { get; set; }
        public double BlinkerTimer { get; set; } = 0d;
        public bool IsSelectionMode => IndexMax - IndexMin > 0;
        public float CursorRelativeThickness { get; set; } = CURSOR_DEFAULT_RELATIVE_THICKNESS;
        public Vector2? BlinkerRelativeDrawPositionMin { get; set; }
        public Vector2? BlinkerRelativeDrawPositionMax { get; set; }
        public bool IsBlinkerAvailable => BlinkerRelativeDrawPositionMin.HasValue && BlinkerRelativeDrawPositionMax.HasValue;
    }

    private enum NavigationDelayType
    {
        NoDelay,
        InitialDelay,
        RepeatedDelay
    }
}