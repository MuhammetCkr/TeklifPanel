﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeklifPanel.Entity;

namespace TeklifPanel.Business.Abstract
{
    public interface ICategoryService : IService<Category>
    {
        Task<List<Category>> GetCategoriesAsync(int companyd);
        Task<bool> DeleteCategoryAsync(int companyd);
    }
}
