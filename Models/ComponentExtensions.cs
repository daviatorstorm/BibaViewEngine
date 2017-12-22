using BibaViewEngine.Compiler;
using BibaViewEngine.Models;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BibaViewEngine.Extensions
{
    public static class ComponentExtensions
    {
        public static T CreateComponent<T>(this IServiceProvider provider, HtmlNode node = null) where T : Component
        {
            var component = provider.GetRequiredService<T>();

            return (T)InitComponent(provider, component, node);
        }

        public static Component CreateComponent(this IServiceProvider provider, Type componentType, HtmlNode node = null)
        {
            var component = (Component)provider.GetRequiredService(componentType);

            return InitComponent(provider, component, node);
        }

        private static Component InitComponent(IServiceProvider provider, Component component, HtmlNode node = null)
        {
            component.Props = provider.GetRequiredService<BibaViewEngineProperties>();
            component.HtmlElement = node;
            component._compiler = provider.GetRequiredService<BibaCompiler>();
            component.StartInitEvent();

            return component;
        }
    }
}
