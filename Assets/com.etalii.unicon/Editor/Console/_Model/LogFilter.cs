namespace EtAlii.UniCon.Editor
{
    using System;
    using System.IO;
    using Serilog.Expressions;
    using UniRx;
#if UNICON_LIFETIME_DEBUG            
    using UnityEngine;
#endif
    // ReSharper disable once InconsistentNaming
    public class LogFilter
    {
        /// <summary>
        /// The name with which the custom filter should be shown in the Filter panel. 
        /// </summary>
        public readonly ReactiveProperty<string> Name = new();
        private string _name;
        
        /// <summary>
        /// Set this property to true to apply the custom filter to the stream of LogEvents.
        /// </summary>
        public readonly ReactiveProperty<bool> IsActive = new();
        private bool _isActive = true;
        
        /// <summary>
        /// Set this property to true to indicate that the custom filter is being edited.
        /// </summary>
        public readonly ReactiveProperty<bool> IsEditing = new(false);
        
        /// <summary>
        /// The expression that should be run on the stream of event signals.
        /// This gets compiled into the <see cref="CompiledExpression"/>.
        /// </summary>
        public readonly ReactiveProperty<string> Expression = new ();
        private string _expression;
        
        public readonly ReactiveProperty<CompiledExpression> CompiledExpression = new ();
        
        /// <summary>
        /// Any error that occurred when compiling the <see cref="Expression"/> into the <see cref="CompiledExpression"/>.
        /// Null indicates no error and that the CompiledExpression can be used.
        /// </summary>
        public string Error { get; private set; }

        // ReSharper disable once NotAccessedField.Local
        private readonly CompositeDisposable _disposable = new ();

        private readonly StaticMemberNameResolver _helperMethodNameResolver = new(typeof(ExpressionHelpers));

        private readonly TimeSpan _throttle = TimeSpan.FromMilliseconds(100);

        public void Bind()
        {
#if UNICON_LIFETIME_DEBUG            
            Debug.Log($"[UNICON] {GetType().Name}.{nameof(Bind)}()");

            if (_disposable.Count > 0) throw new InvalidOperationException($"{GetType().Name} already bound");
#endif
            Name.Value = _name;
            IsActive.Value = _isActive;
            Expression.Value = _expression;

            Name.Subscribe(value => _name = value).AddTo(_disposable);
            IsActive.Subscribe(value => _isActive = value).AddTo(_disposable);
            Expression.Subscribe(value => { _expression = value; Update(); }).AddTo(_disposable);

            Observable
                .Merge( new []
                {
                    Name.Select(_ => true),
                    IsActive.Select(_ => true),
                    Expression.Select(_ => true),
                })
                .Throttle(_throttle)
                .Subscribe(_ => { if(!IsEditing.Value) UserSettings.instance.SaveWhenNeeded(); });
        }
        
        private void Update()
        {
            try
            {
                // Let's not start complaining if there is no expression to compile.
                var expressionToCompile = Expression.Value;
                if (string.IsNullOrWhiteSpace(Expression.Value))
                {
                    Error = null;
                    CompiledExpression.Value = null;
                    return;
                }


                if (SerilogExpression.TryCompile(
                        expression: expressionToCompile, 
                        nameResolver: _helperMethodNameResolver, 
                        result: out var compiledExpression, 
                        error: out var error,
                        formatProvider: null))
                {
                    // `compiledExpression` is a function that can be executed against `LogEvent`s:
                    // `result` will contain a `LogEventPropertyValue`, or `null` if the result of evaluating the
                    // expression is undefined (for example if the event has no `RequestPath` property).
                    CompiledExpression.Value = compiledExpression;
                }
                else
                {
                    // `error` describes a syntax error.
                    Error = error;
                    CompiledExpression.Value = null;
                }
                
            }
            catch (Exception e)
            {
                Error = e.Message;
                CompiledExpression.Value = null;
            }
        }

        public string Serialize()
        {
            using var stream = new MemoryStream();
            using var writer = new BinaryWriter(stream);

            writer.Write(_name ?? string.Empty);
            writer.Write(_expression);
            writer.Write(_isActive);
                    
            writer.Flush();
            stream.Flush();

            var bytes = stream.ToArray();
            return Convert.ToBase64String(bytes);
        }

        public void Deserialize(string f)
        {
            var bytes = Convert.FromBase64String(f);
            using var stream = new MemoryStream(bytes);
            using var reader = new BinaryReader(stream);

            _name = reader.ReadString();
            _expression = reader.ReadString();
            _isActive = reader.ReadBoolean();
            Bind();
        }
    }
}
