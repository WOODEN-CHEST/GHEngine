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
    public bool IsTextInserted
    {
        get => _isTextInserted;
        set
        {
            if (_isTextInserted == value)
            {
                return;
            }
            _isTextInserted = value;
            _cursor.BlinkerTimer = 0d;
            InvalidateBlinkerCache();
        }
    }

    public TimeSpan NavigationDelayInitial { get; set; } = NAVIGATION_DELAY_INITIAL_DEFAULT;
    public TimeSpan NavigationDelayRepeat { get; set; } = NAVIGATION_DELAY_REPEAT_DEFAULT;


    // Private fields.
    private bool _isTypingEnabled = false;
    private readonly IUserInput _userInput;
    private bool _isFocused = false;
    private TextCursor _cursor = new();
    private bool _isTextInserted = false;

    private double _navigationDelaySeconds = 0d;
    private NavigationDelayType _navigationDelayType = NavigationDelayType.NoDelay;


    // Constructors.
    public WritableTextBox(IUserInput userInput)
    {
        _userInput = userInput ?? throw new ArgumentNullException(nameof(userInput));
    }


    // Private methods.
    /* Cursor. */
    private Vector2 RotateWindowPosition(Vector2 windowPosition)
    {
        return Vector2.Rotate(GHMath.GetWindowAdjustedVector(windowPosition - Position, _userInput.InputAreaRatio), -Rotation) + Position;
    }

    private int PositionToNearestTextIndex(Vector2 windowPosition)
    {
        Vector2 RotatedWindowPosition = RotateWindowPosition(windowPosition);
        int TextIndex = 0;

        Vector2 CurPosition = Position - (Origin * GHMath.GetWindowAdjustedVector(DrawSize, _userInput.InputAreaRatio));
        for (int DrawLineIndex = 0; DrawLineIndex < DrawLines.Length; DrawLineIndex++)
        {
            TextIndex += DrawLineIndex != 0 ? 1 : 0;
            DrawLine Line = DrawLines[DrawLineIndex];
            Vector2 LineDrawSize = GHMath.GetWindowAdjustedVector(Line.DrawSize, _userInput.InputAreaRatio);
            if ((RotatedWindowPosition.Y > CurPosition.Y + LineDrawSize.Y) && (DrawLineIndex != DrawLines.Length - 1))
            {
                CurPosition.Y += LineDrawSize.Y;
                TextIndex += Line.Components.Select(component => component.Text.Length).Sum();
                continue;
            }

            return TextIndex + GetNearestTextIndexInDrawLine(Line, CurPosition, RotatedWindowPosition);
        }

        return 0;
    }

    private int GetNearestTextIndexInDrawLine(DrawLine line, Vector2 startPosition, Vector2 rotatedWindowPosition)
    {
        Vector2 CurPosition = startPosition;
        TextComponent TargetComponent = line.Components[0];
        int LineCharIndex = 0;

        for (int ComponentIndex = 0; ComponentIndex < line.Components.Count; ComponentIndex++)
        {
            TargetComponent = line.Components[ComponentIndex];
            if (CurPosition.X + TargetComponent.DrawSize.X >= rotatedWindowPosition.X)
            {
                break;
            }
            LineCharIndex += TargetComponent.Text.Length;
            CurPosition.X += GHMath.GetWindowAdjustedVector(TargetComponent.DrawSize, _userInput.InputAreaRatio).X;
        }

        int ComponentCharIndex = 0;
        while ((CurPosition.X < rotatedWindowPosition.X) && (ComponentCharIndex < TargetComponent.Text.Length))
        {
            Vector2 CharDrawSize = TargetComponent.CalculateDrawSize(TargetComponent.Text[ComponentCharIndex].ToString());
            CharDrawSize = GHMath.GetWindowAdjustedVector(CharDrawSize, _userInput.InputAreaRatio);
            if (CurPosition.X + (CharDrawSize.X * 0.5f) > rotatedWindowPosition.X)
            {
                break;
            }
            ComponentCharIndex++;
            CurPosition.X += CharDrawSize.X;
        }

        return LineCharIndex + ComponentCharIndex;
    }

    private bool IsBlinkerCacheInvalid(float curAspectRatio)
    {
        return (_cursor.BlinkerRelativeDrawPositionMin == null)
            || (_cursor.BlinkerRelativeDrawPositionMax == null)
            || (_cursor.IndexTargetMin == null)
            || (_cursor.IndexTargetMax == null)
            || (curAspectRatio != _cursor.BlinkerCacheAspectRatio);
    }


    private void EnsureBlinkerRenderCache(float aspectRatio)
    {
        if (!IsBlinkerCacheInvalid(aspectRatio))
        {
            return;
        }

        _cursor.BlinkerCacheAspectRatio = aspectRatio;
        int TextIndex = 0;
        for (int DrawLineIndex = 0; DrawLineIndex < DrawLines.Length; DrawLineIndex++)
        {
            DrawLine Line = DrawLines[DrawLineIndex];
            for (int ComponentIndex = 0; ComponentIndex < Line.Components.Count; ComponentIndex++)
            {
                TextComponent Component = Line.Components[ComponentIndex];
                if ((_cursor.IndexMax > Component.Text.Length + TextIndex + DrawLineIndex) && (DrawLineIndex < DrawLines.Length - 1))
                {
                    TextIndex += Component.Text.Length;
                    continue;
                }

                int FinalTextIndex = _cursor.IndexMax - TextIndex - DrawLineIndex;
                Vector2 DrawPositionOffset = GetDrawPosition(DrawLines, Line, Component) - Position;
                Vector2 ComponentDrawSize = Component.CalculateDrawSize(Component.Text.Substring(0, FinalTextIndex));

                if (IsTextInserted && (FinalTextIndex < Component.Text.Length))
                {
                    ComponentDrawSize += new Vector2(Component.CalculateDrawSize(Component.Text[FinalTextIndex].ToString()).X / 2f, 0f);
                }

                Vector2 OriginOffset = Origin * GHMath.GetWindowAdjustedVector(DrawSize, aspectRatio);

                Vector2 MinPoint = Position + GHMath.GetWindowAdjustedVector(DrawPositionOffset
                    + new Vector2(ComponentDrawSize.X, 0f), aspectRatio) - OriginOffset;

                Vector2 MaxPoint = MinPoint + GHMath.GetWindowAdjustedVector(new Vector2(0f, Component.FontSize), aspectRatio);

                _cursor.BlinkerRelativeDrawPositionMin = new Vector2(MinPoint.X, MinPoint.Y);
                _cursor.BlinkerRelativeDrawPositionMax = new Vector2(MaxPoint.X, MaxPoint.Y);

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
        for (int ComponentIndex = 0; ComponentIndex < ComponentCount; ComponentIndex++)
        {
            TextComponent Component = this[ComponentIndex];
            if ((targetIndex < Component.Text.Length + CurrentIndex) || (ComponentIndex >= ComponentCount - 1))
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

    private float GetBLinkerThickness(TextComponent targetedComponent, int textIndex)
    {
        float Thickness = targetedComponent.FontSize * CursorRelativeThickness;
        if (IsTextInserted && textIndex < targetedComponent.Text.Length)
        {
            Thickness = Math.Max(Thickness, targetedComponent.CalculateDrawSize(
                targetedComponent.Text[textIndex].ToString()).X);
        }
        return Thickness;
    }

    private void RenderBlinker(IRenderer renderer, IProgramTime time)
    {
        EnsureBlinkerRenderCache(renderer.AspectRatio);

        CursorTarget CurrentTarget = _cursor.IndexTargetMax!;
        TextComponent TargetedComponent = CurrentTarget.Component;

        Vector2 BlinkerPosMin = _cursor.BlinkerRelativeDrawPositionMin!.Value;
        Vector2 BlinkerPosMax = _cursor.BlinkerRelativeDrawPositionMax!.Value;

        Vector2 MinPosFinal = Vector2.Rotate((BlinkerPosMin - Position) * new Vector2(renderer.AspectRatio, 1f), Rotation);
        Vector2 MaxPosFinal = Vector2.Rotate((BlinkerPosMax - Position) * new Vector2(renderer.AspectRatio, 1f), Rotation);

        MaxPosFinal.X /= renderer.AspectRatio;
        MinPosFinal.X /= renderer.AspectRatio;

        MaxPosFinal += Position;
        MinPosFinal += Position;

        renderer.DrawLine(
            GetCursorColor(TargetedComponent.Mask),
            MinPosFinal,
            MaxPosFinal,
            GetBLinkerThickness(TargetedComponent, CurrentTarget.TextIndex),
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

        for (int i = _cursor.IndexMax; i > 0;)
        {
            i--;
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
        if (_userInput.AreKeysDown(Keys.Up))
        {

        }
        if (_userInput.AreKeysDown(Keys.Down))
        {

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

    /* Typing. */
    private void TypeText(string text)
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
        string CombinedText = $"{LeftPart}{text}{RightPart}";

        Target.Component.Text = CombinedText;

        MoveSingleCursor(1);
        ResetBlinkTimer();
    }

    private void DeleteText(int minIndex, int maxIndexExclusive, int cursorMove)
    {
        if (_cursor.IndexTargetMax == null)
        {
            UpdateCursorTargets();
        }
        CursorTarget Target = _cursor.IndexTargetMin!;

        string OriginalText = Target.Component.Text;

        int LeftIndex = Math.Max(0, minIndex);
        int RightIndex = Math.Min(maxIndexExclusive, OriginalText.Length);

        string LeftPart = OriginalText.Substring(0, LeftIndex);
        string RightPart = OriginalText.Substring(RightIndex, OriginalText.Length - RightIndex);
        string CombinedText = $"{LeftPart}{RightPart}";

        Target.Component.Text = CombinedText;

        MoveSingleCursor(cursorMove);
        ResetBlinkTimer();
    }

    private void ExecuteKeyPress(char character, Keys key)
    {
        if (IsSelectionMade)
        {
            DeleteText(_cursor.IndexMin, _cursor.IndexMax, -1);
        }

        if (key == Keys.Back)
        {
            DeleteText(_cursor.IndexMin - 1, _cursor.IndexMax, -1);
        }
        else if (key == Keys.Delete)
        {
            DeleteText(_cursor.IndexMin, _cursor.IndexMax + 1, 0);
        }
        else if (key == Keys.Enter)
        {
            TypeText("\n");
        }
        else if (key == Keys.Tab)
        {
            TypeText("    ");
        }
        else
        {
            TypeText(character.ToString());
        }
    }

    // Inherited methods.
    public void Update(IProgramTime time)
    {
        _cursor.BlinkerTimer = (_cursor.BlinkerTimer + time.PassedTime.TotalSeconds) % (CursorBlinkDelay.TotalSeconds * 2d);

        if (!IsFocused)
        {
            return;
        }

        if (IsKeyNavigationAllowed)
        {
            NavigateKeyboard(time);
        }
        if (_userInput.WereKeysJustPressed(Keys.Insert))
        {
            IsTextInserted = !IsTextInserted;
        }

        if (_userInput.WereMouseButtonsJustPressed(MouseButton.Left))
        {
            SetSingleSelection(PositionToNearestTextIndex(_userInput.VirtualMousePositionCurrent));
        }
    }

    public override void Render(IRenderer renderer, IProgramTime time)
    {
        bool ShouldRenderBlinker = IsFocused && IsTypingEnabled && (_cursor.BlinkerTimer <= CursorBlinkDelay.TotalSeconds);
        if (IsTextInserted && ShouldRenderBlinker)
        {
            RenderBlinker(renderer, time);
        }

        base.Render(renderer, time);

        if (!IsTextInserted && ShouldRenderBlinker)
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
        public float BlinkerCacheAspectRatio { get; set; } = 0f;
        public bool IsBlinkerAvailable => BlinkerRelativeDrawPositionMin.HasValue && BlinkerRelativeDrawPositionMax.HasValue;
    }

    private enum NavigationDelayType
    {
        NoDelay,
        InitialDelay,
        RepeatedDelay
    }
}