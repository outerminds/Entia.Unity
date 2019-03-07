namespace Entia.Modules
{
    public static class WorldExtensions
    {
        /// <summary>
        /// Gets or create the <see cref="Modules.Delegates"/> module.
        /// </summary>
        /// <param name="world">The world.</param>
        /// <returns>The module.</returns>
        public static Delegates Delegates(this World world)
        {
            if (world.TryGet<Delegates>(out var module)) return module;
            world.Set(module = new Delegates());
            return module;
        }
    }
}