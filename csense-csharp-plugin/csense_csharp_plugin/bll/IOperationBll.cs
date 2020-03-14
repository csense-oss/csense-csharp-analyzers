using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Text;

namespace csenseCsharpAnalyzers.bll {
    public static class IOperationBll {
        public static T? ParentOfType<T>(
            this IOperation op)
            where T : class, IOperation {
            var currentParent = op.Parent;
            while (currentParent != null) {
                if (currentParent is T result) {
                    return result;
                }
                currentParent = currentParent.Parent;
            }
            return null;
        }

        public static void ForeachDescendantsOfType<T>(
            this IOperation op,
            Action<T> action)
            where T : class, IOperation {
            op.ForEachChildRecursivly((child) => {
                if (child is T realType) {
                    action(realType);
                }
                return true;
            });
        }

        public static bool AnyDescendantsOfType<T>(
            this IOperation op,
            Func<T, bool> action)
            where T : class, IOperation {
            var any = false;
            op.ForEachChildRecursivly((child) => {
                if (child is T realType) {
                    if (action(realType)) {
                        any = true;
                        return false;
                    }
                }
                return true;
            });
            return any;
        }

        //todo optimizes
        /// <summary>
        /// 
        /// </summary>
        /// <param name="op"></param>
        /// <param name="action">returns false if we are to break execution, true means go on.</param>
        /// <returns></returns>
        public static bool ForEachChildRecursivly(
            this IOperation op,
            Func<IOperation, bool> action) {
            foreach (var item in op.Children) {
                if(!ForEachChildRecursivly(item, action)) {
                    return false;
                }
                if (!action(item)) {
                    return false;
                }
            }
            return true;
        }
    }

}


