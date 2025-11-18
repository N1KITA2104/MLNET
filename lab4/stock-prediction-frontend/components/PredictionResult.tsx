'use client';

import { PredictionResponse } from '@/app/page';
import { useEffect, useState } from 'react';

interface PredictionResultProps {
  prediction: PredictionResponse;
  warning?: string;
}

export default function PredictionResult({ prediction, warning }: PredictionResultProps) {
  const [animatedValue, setAnimatedValue] = useState(0);

  useEffect(() => {
    const duration = 800;
    const steps = 40;
    const increment = prediction.predictedClose / steps;
    let current = 0;
    let step = 0;

    const timer = setInterval(() => {
      step++;
      current += increment;
      if (step >= steps) {
        setAnimatedValue(prediction.predictedClose);
        clearInterval(timer);
      } else {
        setAnimatedValue(current);
      }
    }, duration / steps);

    return () => clearInterval(timer);
  }, [prediction.predictedClose]);

  return (
    <div className="bg-white border-3 border-black rounded-lg p-6 h-full flex flex-col">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-2xl font-black text-black">RESULT</h2>
        <span className="px-3 py-1.5 bg-black text-white rounded-lg text-sm font-black uppercase">
          {prediction.modelType}
        </span>
      </div>
      
      <div className="flex-1 flex flex-col space-y-4">
        {/* Main Price Display */}
        <div className="bg-black border-3 border-black rounded-lg p-5">
          <div className="flex items-center justify-between mb-3">
            <span className="text-sm font-black text-white uppercase">Predicted Price</span>
            <div className="flex items-center space-x-2">
              <div className="w-2.5 h-2.5 bg-white rounded-full animate-pulse"></div>
              <span className="text-sm text-white font-bold">LIVE</span>
            </div>
          </div>
          <div className="text-5xl font-black text-white">
            ${animatedValue.toFixed(2)}
          </div>
          <div className="text-sm text-gray-300 font-bold mt-2">
            ML.NET {prediction.modelType} Model
          </div>
        </div>

        {/* Model Info */}
        <div className="bg-white border-3 border-black rounded-lg p-4">
          <div className="flex items-center justify-between">
            <div>
              <div className="text-sm text-black font-black uppercase mb-1">Model</div>
              <div className="text-base font-black text-black">
                {prediction.modelType === 'ONNX' ? 'ONNX' : 'ZIP'}
              </div>
            </div>
            <div className="w-12 h-12 bg-black border-3 border-black rounded-lg flex items-center justify-center">
              {prediction.modelType === 'ONNX' ? (
                <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" strokeWidth={3}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M9 12l2 2 4-4m5.618-4.016A11.955 11.955 0 0112 2.944a11.955 11.955 0 01-8.618 3.04A12.02 12.02 0 003 9c0 5.591 3.824 10.29 9 11.622 5.176-1.332 9-6.03 9-11.622 0-1.042-.133-2.052-.382-3.016z" />
                </svg>
              ) : (
                <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24" strokeWidth={3}>
                  <path strokeLinecap="round" strokeLinejoin="round" d="M5 8h14M5 8a2 2 0 110-4h14a2 2 0 110 4M5 8v10a2 2 0 002 2h10a2 2 0 002-2V8m-9 4h4" />
                </svg>
              )}
            </div>
          </div>
        </div>

        {warning && (
          <div className="bg-white border-3 border-black rounded-lg p-3">
            <div className="flex items-start">
              <svg className="w-5 h-5 text-black flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={3}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
              </svg>
              <p className="text-xs text-black font-bold leading-tight ml-2">{warning}</p>
            </div>
          </div>
        )}

        <div className="pt-3 border-t-3 border-black mt-auto">
          <p className="text-sm text-black font-bold leading-tight">
            ML model prediction. Not financial advice.
          </p>
        </div>
      </div>
    </div>
  );
}
