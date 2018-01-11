﻿using BibaViewEngine.Compiler;
using BibaViewEngine.Models;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BibaViewEngine.Extensions
{
    public static class ComponentExtensions
    {
        public static T CreateComponent<T>(this IServiceProvider provider, HtmlNode node = null) where T : Component
        {
            var component = provider.GetRequiredService<T>();
            var context = provider.GetRequiredService<HttpContextAccessor>().HttpContext;

            return (T)InitComponent(provider, component, node, context);
        }

        public static Component CreateComponent(this IServiceProvider provider, Type componentType, HtmlNode node = null)
        {
            var component = (Component)provider.GetRequiredService(componentType);
            var context = provider.GetRequiredService<HttpContextAccessor>().HttpContext;

            return InitComponent(provider, component, node, context);
        }

        private static Component InitComponent(IServiceProvider provider, Component component, HtmlNode node = null, HttpContext context = null)
        {
            component.Props = provider.GetRequiredService<BibaViewEngineProperties>();
            component.HtmlElement = node;
            component._compiler = provider.GetRequiredService<BibaCompiler>();
            component.HttpContext = context;
            component.StartInitEvent();

            return component;
        }
    }
}
