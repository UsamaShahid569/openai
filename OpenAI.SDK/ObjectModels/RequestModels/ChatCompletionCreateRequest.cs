﻿using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.SharedModels;

namespace OpenAI.ObjectModels.RequestModels;

public class ChatCompletionCreateRequest : IModelValidate, IOpenAiModels.ITemperature, IOpenAiModels.IModel, IOpenAiModels.IUser
{
    public enum ResponseFormats
    {
        Text,
        Json
    }

    /// <summary>
    ///     The messages to generate chat completions for, in the chat format.
    ///     The main input is the messages parameter. Messages must be an array of message objects, where each object has a
    ///     role (either “system”, “user”, or “assistant”) and content (the content of the message). Conversations can be as
    ///     short as 1 message or fill many pages.
    /// </summary>
    [JsonPropertyName("messages")]
    public IList<ChatMessage> Messages { get; set; }

    /// <summary>
    ///     A list of functions the model may generate JSON inputs for.
    /// </summary>
    [JsonIgnore]
    public IList<FunctionDefinition>? Functions { get; set; }

    [JsonIgnore] public object? FunctionsAsObject { get; set; }

    [JsonPropertyName("functions")]
    public object? FunctionCalculated
    {
        get
        {
            if (FunctionsAsObject != null && Functions != null)
            {
                throw new ValidationException("FunctionAsObject and Functions can not be assigned at the same time. One of them is should be null.");
            }

            return Functions ?? FunctionsAsObject;
        }
    }

    /// <summary>
    ///     An alternative to sampling with temperature, called nucleus sampling, where the model considers the results of the
    ///     tokens with top_p probability mass. So 0.1 means only the tokens comprising the top 10% probability mass are
    ///     considered.
    ///     We generally recommend altering this or temperature but not both.
    /// </summary>
    [JsonPropertyName("top_p")]
    public float? TopP { get; set; }

    /// <summary>
    ///     How many chat completion choices to generate for each input message.
    /// </summary>
    [JsonPropertyName("n")]
    public int? N { get; set; }

    /// <summary>
    ///     If set, partial message deltas will be sent, like in ChatGPT. Tokens will be sent as data-only server-sent events
    ///     as they become available, with the stream terminated by a data: [DONE] message.
    /// </summary>
    [JsonPropertyName("stream")]
    public bool? Stream { get; set; }

    /// <summary>
    ///     Up to 4 sequences where the API will stop generating further tokens. The returned text will not contain the stop
    ///     sequence.
    /// </summary>
    [JsonIgnore]
    public string? Stop { get; set; }

    /// <summary>
    ///     Up to 4 sequences where the API will stop generating further tokens. The returned text will not contain the stop
    ///     sequence.
    /// </summary>
    [JsonIgnore]
    public IList<string>? StopAsList { get; set; }

    [JsonPropertyName("stop")]
    public IList<string>? StopCalculated
    {
        get
        {
            if (Stop != null && StopAsList != null)
            {
                throw new ValidationException("Stop and StopAsList can not be assigned at the same time. One of them is should be null.");
            }

            if (Stop != null)
            {
                return new List<string> { Stop };
            }

            return StopAsList;
        }
    }

    /// <summary>
    ///     The maximum number of tokens allowed for the generated answer. By default, the number of tokens the model can
    ///     return will be (4096 - prompt tokens).
    /// </summary>
    /// <see href="https://platform.openai.com/docs/api-reference/completions/create#completions/create-max_tokens" />
    [JsonPropertyName("max_tokens")]
    public int? MaxTokens { get; set; }

    /// <summary>
    ///     Number between -2.0 and 2.0. Positive values penalize new tokens based on whether they appear in the text so far,
    ///     increasing the model's likelihood to talk about new topics.
    /// </summary>
    /// <seealso href="https://platform.openai.com/docs/api-reference/parameter-details" />
    [JsonPropertyName("presence_penalty")]
    public float? PresencePenalty { get; set; }


    /// <summary>
    ///     Number between -2.0 and 2.0. Positive values penalize new tokens based on their existing frequency in the text so
    ///     far, decreasing the model's likelihood to repeat the same line verbatim.
    /// </summary>
    /// <seealso href="https://platform.openai.com/docs/api-reference/parameter-details" />
    [JsonPropertyName("frequency_penalty")]
    public float? FrequencyPenalty { get; set; }

    /// <summary>
    ///     Modify the likelihood of specified tokens appearing in the completion.
    ///     Accepts a json object that maps tokens(specified by their token ID in the GPT tokenizer) to an associated bias
    ///     value from -100 to 100. You can use this tokenizer tool (which works for both GPT-2 and GPT-3) to convert text to
    ///     token IDs. Mathematically, the bias is added to the logits generated by the model prior to sampling. The exact
    ///     effect will vary per model, but values between -1 and 1 should decrease or increase likelihood of selection; values
    ///     like -100 or 100 should result in a ban or exclusive selection of the relevant token.
    ///     As an example, you can pass { "50256": -100}
    ///     to prevent the endoftext token from being generated.
    /// </summary>
    /// <seealso href="https://platform.openai.com/tokenizer?view=bpe" />
    [JsonPropertyName("logit_bias")]
    public object? LogitBias { get; set; }


    /// <summary>
    ///     String or object. Controls how the model responds to function calls.
    ///     "none" means the model does not call a function, and responds to the end-user.
    ///     "auto" means the model can pick between an end-user or calling a function.
    ///     "none" is the default when no functions are present. "auto" is the default if functions are present.
    ///     Specifying a particular function via {"name": "my_function"} forces the model to call that function.
    ///     (Note: in C# specify that as:
    ///     FunctionCall = new Dictionary&lt;string, string&gt; { { "name", "my_function" } }
    ///     ).
    /// </summary>
    [JsonPropertyName("function_call")]
    public object? FunctionCall { get; set; }

    /// <summary>
    ///     The format that the model must output. Used to enable JSON mode.
    ///     Must be one of "text" or "json_object".<br />
    ///     <see cref="StaticValues.CompletionStatics.ResponseFormat" /><br />
    ///     <example>
    ///         Sample Usage:<br />
    ///         new ResponseFormat { Type = StaticValues.CompletionStatics.ResponseFormat.Json }
    ///     </example>
    /// </summary>
    [JsonPropertyName("response_format")]
    public ResponseFormat? ResponseFormat { get; set; }

    /// <summary>
    ///     The format that the model must output. Used to enable JSON mode.
    ///     Must be one of "text" or "json_object".
    /// </summary>
    /// <example>
    ///     This example shows how to set the ChatResponseFormat to JSON:
    ///     <code>
    ///         var chatResponse = new ChatResponse
    ///         {
    ///             ChatResponseFormat = ChatResponseFormats.Json
    ///         };
    ///     </code>
    /// </example>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when an unsupported <see cref="ResponseFormats" /> value is provided.
    /// </exception>
    /// <exception cref="ValidationException">
    ///     Thrown when <see cref="ResponseFormat" /> is already set.
    /// </exception>
    [JsonIgnore]
    public ResponseFormats? ChatResponseFormat
    {
        set
        {
            if (value == null) return;
            if (ResponseFormat?.Type != null)
            {
                throw new ValidationException("ResponseFormat and ChatResponseFormat can not be assigned at the same time. One of them is should be null.");
            }

            ResponseFormat = new ResponseFormat
            {
                Type = value switch
                {
                    ResponseFormats.Json => StaticValues.CompletionStatics.ResponseFormat.Json,
                    ResponseFormats.Text => StaticValues.CompletionStatics.ResponseFormat.Text,
                    _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
                }
            };
        }
    }

    /// <summary>
    ///     This feature is in Beta. If specified, our system will make a best effort to sample deterministically, such that
    ///     repeated requests with the same seed and parameters should return the same result. Determinism is not guaranteed,
    ///     and you should refer to the system_fingerprint response parameter to monitor changes in the backend.
    /// </summary>
    [JsonPropertyName("seed")]
    public int? Seed { get; set; }

    /// <summary>
    ///     ID of the model to use. For models supported see <see cref="OpenAI.ObjectModels.Models" /> start with <c>Gpt_</c>
    /// </summary>
    [JsonPropertyName("model")]
    public string? Model { get; set; }

    public IEnumerable<ValidationResult> Validate()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     What sampling temperature to use, between 0 and 2. Higher values like 0.8 will make the output more random, while
    ///     lower values like 0.2 will make it more focused and deterministic.
    ///     We generally recommend altering this or top_p but not both.
    /// </summary>
    [JsonPropertyName("temperature")]
    public float? Temperature { get; set; }

    /// <summary>
    ///     A unique identifier representing your end-user, which can help OpenAI to monitor and detect abuse. Learn more.
    /// </summary>
    [JsonPropertyName("user")]
    public string User { get; set; }
}