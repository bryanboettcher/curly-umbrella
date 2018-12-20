
namespace HWI.Internal.Translation
{
    /// <summary>
    /// Defines a common behavior for migrating one type of object to another
    /// </summary>
    /// <typeparam name="TIn">The source type of object</typeparam>
    /// <typeparam name="TOut">The destination type of object</typeparam>
    /// <remarks>Translators are assumed to be both simple and fast objects, without
    /// contacting external systems like databases or filesystems</remarks>
    public interface IObjectTranslator<in TIn, TOut> where TOut : new()
    {
        /// <summary>
        /// Creates a new TOut and maps properties over
        /// </summary>
        /// <param name="input">The original item to be translated</param>
        /// <returns>A new instance of <typeparam name="TOut">TOut</typeparam></returns>
        TOut Translate(TIn input);

        /// <summary>
        /// Modifies an existing TOut with new values.  May be useful in the scenario
        /// when an object goes through multiple translation steps.  <param name="existing">existing</param>
        /// may be modified in this process.
        /// </summary>
        /// <param name="input">The original item to be translated</param>
        /// <param name="existing">The previously-mapped item</param>
        /// <returns>A modified instance of <typeparam name="TOut">TOut</typeparam></returns>
        TOut Translate(TIn input, TOut existing);
    }
}