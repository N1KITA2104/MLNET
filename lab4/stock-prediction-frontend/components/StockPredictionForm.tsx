'use client';

import { useState, FormEvent } from 'react';

interface StockPredictionFormProps {
  onSubmit: (data: {
    open: number;
    high: number;
    low: number;
    volume: number;
  }) => void;
  loading: boolean;
}

export default function StockPredictionForm({ onSubmit, loading }: StockPredictionFormProps) {
  const [formData, setFormData] = useState({
    open: '',
    high: '',
    low: '',
    volume: '',
  });

  const [errors, setErrors] = useState<Record<string, string>>({});

  const handleSubmit = (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setErrors({});
    
    const data = {
      open: parseFloat(formData.open),
      high: parseFloat(formData.high),
      low: parseFloat(formData.low),
      volume: parseFloat(formData.volume),
    };

    const newErrors: Record<string, string> = {};

    if (isNaN(data.open) || data.open <= 0) {
      newErrors.open = 'Invalid number';
    }
    if (isNaN(data.high) || data.high <= 0) {
      newErrors.high = 'Invalid number';
    }
    if (isNaN(data.low) || data.low <= 0) {
      newErrors.low = 'Invalid number';
    }
    if (isNaN(data.volume) || data.volume <= 0) {
      newErrors.volume = 'Invalid number';
    }

    if (data.high < data.low) {
      newErrors.high = 'High must be >= Low';
    }

    if (Object.keys(newErrors).length > 0) {
      setErrors(newErrors);
      return;
    }

    onSubmit(data);
  };

  const handleChange = (field: string, value: string) => {
    setFormData(prev => ({ ...prev, [field]: value }));
    if (errors[field]) {
      setErrors(prev => {
        const newErrors = { ...prev };
        delete newErrors[field];
        return newErrors;
      });
    }
  };

  const inputClasses = (hasError: boolean) => 
    `w-full px-4 py-3 bg-white border-3 border-black rounded-lg text-black font-bold text-base placeholder-gray-500 focus:outline-none focus:ring-2 focus:ring-white focus:ring-offset-2 focus:ring-offset-black disabled:opacity-50 disabled:bg-gray-200`;

  return (
    <div className="bg-white border-3 border-black rounded-lg p-6 h-full flex flex-col">
      <div className="flex items-center mb-4">
        <div className="w-10 h-10 bg-black border-3 border-black rounded-lg flex items-center justify-center mr-3">
          <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" strokeWidth={3}>
            <path strokeLinecap="round" strokeLinejoin="round" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
          </svg>
        </div>
        <h2 className="text-2xl font-black text-black">INPUT DATA</h2>
      </div>

      <form onSubmit={handleSubmit} className="space-y-4 flex-1 flex flex-col">
        <div>
          <label htmlFor="open" className="block text-sm font-black text-black mb-2 uppercase">Open ($)</label>
          <input
            type="number"
            id="open"
            step="0.01"
            min="0"
            value={formData.open}
            onChange={(e) => handleChange('open', e.target.value)}
            className={inputClasses(!!errors.open)}
            placeholder="650.00"
            required
            disabled={loading}
          />
          {errors.open && <p className="mt-1 text-sm text-black font-bold">{errors.open}</p>}
        </div>

        <div>
          <label htmlFor="high" className="block text-sm font-black text-black mb-2 uppercase">High ($)</label>
          <input
            type="number"
            id="high"
            step="0.01"
            min="0"
            value={formData.high}
            onChange={(e) => handleChange('high', e.target.value)}
            className={inputClasses(!!errors.high)}
            placeholder="670.00"
            required
            disabled={loading}
          />
          {errors.high && <p className="mt-1 text-sm text-black font-bold">{errors.high}</p>}
        </div>

        <div>
          <label htmlFor="low" className="block text-sm font-black text-black mb-2 uppercase">Low ($)</label>
          <input
            type="number"
            id="low"
            step="0.01"
            min="0"
            value={formData.low}
            onChange={(e) => handleChange('low', e.target.value)}
            className={inputClasses(!!errors.low)}
            placeholder="640.00"
            required
            disabled={loading}
          />
          {errors.low && <p className="mt-1 text-sm text-black font-bold">{errors.low}</p>}
        </div>

        <div>
          <label htmlFor="volume" className="block text-sm font-black text-black mb-2 uppercase">Volume</label>
          <input
            type="number"
            id="volume"
            step="1"
            min="0"
            value={formData.volume}
            onChange={(e) => handleChange('volume', e.target.value)}
            className={inputClasses(!!errors.volume)}
            placeholder="50000000"
            required
            disabled={loading}
          />
          {errors.volume && <p className="mt-1 text-sm text-black font-bold">{errors.volume}</p>}
        </div>

        <button
          type="submit"
          disabled={loading}
          className="mt-auto w-full bg-black text-white font-black py-3 px-4 rounded-lg border-3 border-black hover:scale-105 transition-transform disabled:opacity-50 disabled:cursor-not-allowed disabled:hover:scale-100 flex items-center justify-center space-x-2 uppercase text-base"
        >
          {loading ? (
            <>
              <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
              </svg>
              <span>ANALYZING...</span>
            </>
          ) : (
            <>
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24" strokeWidth={3}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M13 10V3L4 14h7v7l9-11h-7z" />
              </svg>
              <span>PREDICT</span>
            </>
          )}
        </button>
      </form>
    </div>
  );
}
