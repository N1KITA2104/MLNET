'use client';

import { useState } from 'react';
import StockPredictionForm from '@/components/StockPredictionForm';
import PredictionResult from '@/components/PredictionResult';

export interface PredictionResponse {
  predictedClose: number;
  modelType: string;
  warning?: string;
}

export default function Home() {
  const [prediction, setPrediction] = useState<PredictionResponse | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const handlePrediction = async (data: {
    open: number;
    high: number;
    low: number;
    volume: number;
  }) => {
    setLoading(true);
    setError(null);
    setPrediction(null);

    try {
      const apiUrl = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';
      const response = await fetch(`${apiUrl}/api/prediction/predict`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(data),
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({ error: 'Unknown error' }));
        throw new Error(errorData.error || `HTTP error! status: ${response.status}`);
      }

      const result: PredictionResponse = await response.json();
      setPrediction(result);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to get prediction');
    } finally {
      setLoading(false);
    }
  };

  return (
    <main className="h-screen bg-black text-white overflow-hidden">
      <div className="h-full py-6 px-6">
        <div className="h-full max-w-6xl mx-auto flex flex-col">
          {/* Header */}
          <div className="text-center mb-6">
            <div className="inline-flex items-center justify-center w-16 h-16 bg-white border-3 border-black rounded-xl mb-3">
              <svg className="w-8 h-8 text-black" fill="none" stroke="currentColor" viewBox="0 0 24 24" strokeWidth={3}>
                <path strokeLinecap="round" strokeLinejoin="round" d="M13 7h8m0 0v8m0-8l-8 8-4-4-6 6" />
              </svg>
            </div>
            <h1 className="text-3xl md:text-4xl font-black text-white mb-2">STOCK PRICE PREDICTION</h1>
            <div className="w-32 h-1 bg-white mx-auto"></div>
          </div>

          {/* Main Content */}
          <div className="flex-1 grid lg:grid-cols-2 gap-6 min-h-0">
            {/* Form Section */}
            <div className="min-h-0 overflow-auto">
              <StockPredictionForm onSubmit={handlePrediction} loading={loading} />
            </div>

            {/* Result Section */}
            <div className="min-h-0 overflow-auto">
              {error && (
                <div className="mb-4">
                  <div className="bg-white border-3 border-black rounded-lg p-4">
                    <div className="flex items-start">
                      <svg className="h-5 w-5 text-black flex-shrink-0 mt-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={3}>
                        <path strokeLinecap="round" strokeLinejoin="round" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                      </svg>
                      <div className="ml-2">
                        <p className="text-black font-black">ERROR: {error}</p>
                      </div>
                    </div>
                  </div>
                </div>
              )}
              
              {loading && (
                <div className="bg-white border-3 border-black rounded-lg p-10 text-center">
                  <div className="inline-block animate-spin rounded-full h-14 w-14 border-3 border-black border-t-transparent mb-4"></div>
                  <p className="text-black text-base font-black">ANALYZING...</p>
                </div>
              )}

              {prediction && !loading && (
                <PredictionResult prediction={prediction} warning={prediction.warning} />
              )}

              {!prediction && !loading && !error && (
                <div className="bg-white border-3 border-black rounded-lg p-10 text-center">
                  <svg className="w-14 h-14 text-black mx-auto mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24" strokeWidth={2}>
                    <path strokeLinecap="round" strokeLinejoin="round" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                  <p className="text-black text-sm font-black">ENTER DATA TO PREDICT</p>
                </div>
              )}
            </div>
          </div>
        </div>
      </div>
    </main>
  );
}
